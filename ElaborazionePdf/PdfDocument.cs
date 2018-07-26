using System;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Collections;
using static iTextSharp.text.pdf.AcroFields;
using iTextSharp.text;

namespace ElaborazionePdf
{
	public class PdfDocument
	{
		private string filename;                    //Filename
		private string filename_out;                //Filename for the modified file

		private PdfStamper stamper;
		private PdfReader reader;
		private MemoryStream memoryStream;

		public PdfDocument(string filename)
		{
			//Saving filename
			this.filename = filename;
			//Generating output file name (filename + _modified.pdf)
			filename_out = filename.Substring(0, filename.Length - 4) + "_modified.pdf";
			//Loading file
			LoadFile();
		}

		public void DisposeAll()
		{
			reader.Dispose();
			stamper.Dispose();
			memoryStream.Dispose();
		}

		/*!
		 Loading the input file into memory
		 */
		private void LoadFile()
		{
			memoryStream = new MemoryStream();
			reader = new PdfReader(filename);
			stamper = new PdfStamper(reader, memoryStream);

			stamper.FormFlattening = true;
		}

		/*!
		 METODO 1: ricerca di un acrofield generico per name, 
		 l’oggetto ritornato deve indicare il tipo di acrofield(checkbox, textbox, signaturefield, radiobutton).

		 Note: In case of error or different field type, the method returns -1
		 @param[out] int	Field type
		 @param[in]  string	Field name
		 */
		public int GetAcrofieldType(string fieldName)
		{
			int type;                                               //Type of field

			try
			{
				//Getting fields
				AcroFields form = reader.AcroFields;

				//Analyzing every item
				foreach (KeyValuePair<string, AcroFields.Item> kvp in form.Fields)
				{
					//Cheking if Field type is checkbox or textbox or signaturefield or radiobutton
					switch (type = form.GetFieldType(kvp.Key))
					{
						case AcroFields.FIELD_TYPE_CHECKBOX:
						case AcroFields.FIELD_TYPE_RADIOBUTTON:
						case AcroFields.FIELD_TYPE_SIGNATURE:
						case AcroFields.FIELD_TYPE_TEXT:
							//Reading field name
							string translatedFileName = form.GetTranslatedFieldName(kvp.Key);

							//Comparing filed name with the given name
							if (translatedFileName.Equals(fieldName))
								return type;
							break;
					}
				}
			}
			catch (IOException e)
			{
				//Throwing exception to the caller
				throw e;
			}

			//Returning -1 in case of error or different field type
			return -1;
		}

		/*!
		 METODO 1.1 (aggiuntivo): Ritorna in formato "human readable" il tipo di field passato per parametro
		 l’oggetto ritornato deve indicare il tipo di acrofield(checkbox, textbox, signaturefield, radiobutton)
		 @param[out] string	Field type (human readable)
		 @param[in]  int	Field type
		 */
		public static string GetFormType(int num)
		{
			switch (num)
			{
				case AcroFields.FIELD_TYPE_CHECKBOX:
					return "Checkbox";
				case AcroFields.FIELD_TYPE_COMBO:
					return "Combobox";
				case AcroFields.FIELD_TYPE_LIST:
					return "List";
				case AcroFields.FIELD_TYPE_NONE:
					return "None";
				case AcroFields.FIELD_TYPE_PUSHBUTTON:
					return "Pushbutton";
				case AcroFields.FIELD_TYPE_RADIOBUTTON:
					return "Radiobutton";
				case AcroFields.FIELD_TYPE_SIGNATURE:
					return "Signature";
				case AcroFields.FIELD_TYPE_TEXT:
					return "Text";
				default:
					return "?";
			}
		}

		/*!
		 METODO 2: Flaggare un acrofield di tipo checkbox

		 Locking for a checkbox and checking it
		 @param[out] bool	Operation result
		 */
		public bool FlagCheckbox()
		{
			bool found = false;                                                             //Flag indicating if an unchecked checkbox has been found

			try
			{
				//Getting forms
				AcroFields form = stamper.AcroFields;

				//Analyzing every item
				foreach (KeyValuePair<string, AcroFields.Item> kvp in form.Fields)
				{
					//Checking if the form is checkbox
					if (form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_CHECKBOX)
					{
						//Getting field name
						string name = form.GetTranslatedFieldName(kvp.Key);

						//Getting checkbox's state values (Note: they change according to PDF's language)
						//Note: values[0] is the unchecked value, values[1] is checked value
						string[] values = form.GetAppearanceStates(name);

						//If the box isn't checked, we check it
						if (form.GetField(kvp.Key).Equals(values[0]) || form.GetField(kvp.Key).Length == 0)
						{
							//Changing state and returning true (in case of error it returns false)
							found = form.SetField(name, values[1]);
							break;
						}
					}
				}
				//Disabling flattening because the file could be modified untill the call of save()
				stamper.FormFlattening = false;
			}
			catch (IOException e)
			{
				//Throwing exception to the caller
				throw e;
			}

			return found;
		}

		/*!
		 METODO 3: Sostituire un acrofield di tipo signature con un acrofield di tipo checkbox

		 Locking for a checkbox and checking it
		 @param[out] bool	Operation result
		 */
		public bool SubstituteSignature()
		{
			bool found = false;                                                 //Flag indicating if an unchecked checkbox has been found

			string key = null;
			IList<FieldPosition> positions = null;

			try
			{

				//Getting forms
				AcroFields form = stamper.AcroFields;

				ArrayList arr = new ArrayList();

				//Analyzing every item
				foreach (KeyValuePair<string, AcroFields.Item> kvp in form.Fields)
				{
					arr.Add(kvp.Key);

					//Checking if the form is checkbox
					if (form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_SIGNATURE)
					{
						//Getting field's key
						key = kvp.Key;
						//Getting field's position(s)
						positions = form.GetFieldPositions(form.GetTranslatedFieldName(kvp.Key));

						//Removing field
						form.RemoveField(key);

						//Creating new checkbox with signaturefield's coordinates
						//Note: We're replacing the first occurrence
						RadioCheckField checkbox = new RadioCheckField(stamper.Writer, positions[0].position, "i_was_a_signature_field", "Yes")
						{
							//Setting look
							CheckType = RadioCheckField.TYPE_CHECK,
							Checked = true,
							BorderWidth = BaseField.BORDER_WIDTH_THIN,
							BorderColor = BaseColor.BLACK,
							BackgroundColor = BaseColor.WHITE
						};

						//Adding checbox in signaturefield's page
						stamper.AddAnnotation(checkbox.CheckField, positions[0].page);

						found = true;
						break;
					}
					//Disabling flattening because the file could be modified untill the call of save()
					stamper.FormFlattening = false;
				}
			}
			catch (IOException e)
			{
				//Throwing exception to the caller
				throw e;
			}

			return found;
		}

		/*!
		 METODO 4: Selezionare un acrofield di tipo radiobutton

		 Selecting a radiobutton acrofield
		 @param[out] bool	Operation result
		 */
		public bool SelectRadiobutton()
		{
			bool found = false;                                                 //Flag indicating if an unchecked checkbox has been found
			
			try
			{
					//Getting forms
					AcroFields form = stamper.AcroFields;

					//Analyzing every item
					foreach (KeyValuePair<string, AcroFields.Item> kvp in form.Fields)
					{
						//Cheking if textfield
						if (form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_RADIOBUTTON)
						{
							string name = form.GetTranslatedFieldName(kvp.Key);

							//Getting radiobutton values
							string[] values = form.GetAppearanceStates(kvp.Key);

							//Setting the first value
							found = form.SetField(form.GetTranslatedFieldName(kvp.Key), values[0]);
						}
					}
					//Disabling flattening because the file could be modified untill the call of save()
					stamper.FormFlattening = false;
			}
			catch (IOException e)
			{
				//Throwing exception to the caller
				throw e;
			}

			return found;
		}

		/*!
		 METODO 5: Inserire un testo in un acrofield ti tipo testo

		 Inserting the given text into a textfield
		 @param[out] bool	Operation result
		 */
		public bool InsertTextInField(string fieldName, string text)
		{
			bool found = false;                                                 //Flag indicating if an unchecked checkbox has been found

			try
			{
					//Getting forms
					AcroFields form = stamper.AcroFields;

					//Analyzing every item
					foreach (KeyValuePair<string, AcroFields.Item> kvp in form.Fields)
					{

						//Cheking if textfield
						if (form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_TEXT)
						{
							//Checking field name
							if (form.GetTranslatedFieldName(kvp.Key).Equals(fieldName))
							{
								found = true;
								form.SetField(fieldName, text);
								break;
							}
						}
					}
					//Disabling flattening because the file could be modified untill the call of save()
					stamper.FormFlattening = false;
			}
			catch (IOException e)
			{
				//Throwing exception to the caller
				throw e;
			}

			return found;
		}

		/*!
		 METODO 6: Ottenimento del pdf elaborato

		 Saving the working copy on file
		 */
		public void Save()
		{
			

			var contenuto = memoryStream.ToArray();
			//Console.WriteLine("> " + Encoding.Default.GetString(contenuto));

			using(PdfStamper filestamper = new PdfStamper(new PdfReader(memoryStream), new FileStream(filename_out, FileMode.Create)))
			{
				stamper.FormFlattening = true;
			}
			//Closing stamper
			stamper.Dispose();
			//Closing memorystream
			memoryStream.Dispose();
		}

		private static void CloseProgram()
		{
			Console.WriteLine("\n\nPress any key to exit...");
			Console.ReadLine();
			System.Environment.Exit(1);
		}

		/*************************************************************************
		 * Main di prova
		 *************************************************************************/

		static void Main(string[] args)
		{
			int option = 0;
			PdfDocument p;
			try
			{
				p = new PdfDocument(@"C:\Users\c.veronesi\source\repos\ElaborazionePdf\ElaborazionePdf.UnitTests\TestFiles\Richiesta di adesione e Condizioni relative all'uso della firma elettronica avanzata_checkbox.pdf");
				Console.WriteLine("Opening file file: \"" + p.filename + "\"\n");

				do
				{
					Console.WriteLine("\nMENU\n\n1. Metodo: ricerca di un acrofield generico per name, l’oggetto ritornato deve indicare il tipo di acrofield(checkbox, textbox, signaturefield, radiobutton)\n2. Metodo: per flaggare un acrofield di tipo checkbox\n3. Metodo: per sostituire un acrofield di tipo signature con un acrofield di tipo checkbox\n4. Metodo: per selezionare un acrofield di tipo radiobutton\n5. Metodo: per inserire un testo in un acrofield di tipo testo\n6. Metodo: per ottenere il pdf elaborato\n7. Esci\n\nInserisci la tua scelta:");
					option = Int32.Parse(Console.ReadLine());
					switch (option)
					{
						case 1:
							int fieldType = p.GetAcrofieldType("Nome");
							Console.WriteLine("Looking for field named \"Nome\"...");
							if (fieldType != -1)
								Console.WriteLine("Found type: " + fieldType + " (" + GetFormType(fieldType) + ")");
							else
								Console.WriteLine("Field not found or illegal field type");
							break;
						case 2:
							Console.WriteLine("\nLooking for a checkable Checkbox..." + p.FlagCheckbox());
							break;
						case 3:
							Console.WriteLine("\nLooking for a signature to substitute with a checkbox..." + p.SubstituteSignature());
							break;
						case 4:
							Console.WriteLine("\nLooking for radiobutton to select..." + p.SelectRadiobutton());
							break;
						case 5:
							Console.WriteLine("\nInserting text in field named \"Nome\"..." + p.InsertTextInField("Nome", "Carlo") + "\n\n");
							break;
						case 6:
							p.Save();
							Console.WriteLine("File \"" + p.filename_out + "\" saved.");
							break;
						case 7:
							break;
						default:
							Console.WriteLine("Wrong command");
							break;
					}
				}
				while (option != 7);
			}
			catch (IOException e)
			{
				Console.WriteLine(e);
				CloseProgram();
			}
			Console.ReadLine();
		}

	}
}
