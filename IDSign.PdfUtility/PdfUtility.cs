using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using iTextSharp.text;
using iTextSharp.text.pdf;
using static iTextSharp.text.pdf.AcroFields;

namespace IDSign.PdfUtility
{
	//Logger delegate protorype
	public delegate void LoggerFunction(string text);

	public class PdfUtility : IDisposable
	{
		private string filename;                        //Filename
		private string filename_out;                    //Filename for the modified file
		private PdfStamper stamper = null;
		private PdfReader reader = null;
		private MemoryStream memoryStream = null;
		private bool stamperDisposed = false;           //Indicating if some resources has been disposed				
		static LoggerFunction delegateFunction = null;  //Logger function delegate

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="filename">string Name of the file to opens</param>
		/// <param name="funct">Logger function</param>
		public PdfUtility(string filename, LoggerFunction funct)
		{
			//Delegating logger
			delegateFunction = funct;
			//Saving filename
			this.filename = filename;
			//Generating output file name (filename + _modified.pdf)
			Filename_out = filename.Substring(0, filename.Length - 4) + "_modified.pdf";
			//Loading file
			LoadFile();
		}

		/// <summary>
		/// Getters and setters
		/// </summary>
		public string Filename { get => filename; set => filename = value; }
		public string Filename_out { get => filename_out; set => filename_out = value; }

		/// <summary>
		/// Logger
		/// </summary>
		/// <param name="msg">string Message to display</param>
		private static void Log(string msg)
		{
			//Calling delegate logger function if exists
			delegateFunction?.Invoke(msg);
		}

		/// <summary>
		///  Implementing Dispose()
		/// </summary>
		#region IDisposable Members
		~PdfUtility()
		{
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				//Disposing Stamper if not alerady disposed
				if (!stamperDisposed && stamper != null)
				{
					try
					{
						stamper.Dispose();
					}
					catch (Exception e)
					{
						//Calling logger
						Log("ERROR WHILE DIPOSING STAMPER:\n" + e.ToString());
					}
				}

				//Disposing reader
				if (reader != null)
				{
					try
					{
						reader.Dispose();
					}
					catch (Exception e)
					{
						//Calling logger
						Log("ERROR WHILE DIPOSING READER:\n" + e.ToString());
					}
				}

				//Disposing memoryStream
				if (memoryStream != null)
				{
					try
					{
						memoryStream.Dispose();
					}
					catch (Exception e)
					{
						//Calling logger
						Log("ERROR WHILE DIPOSING MEMORYSTREAM:\n" + e.ToString());
					}
				}
			}
		}
		#endregion

		/// <summary>
		/// Loading the input file into memory
		/// </summary>
		private void LoadFile()
		{
			memoryStream = new MemoryStream();
			reader = new PdfReader(filename);
			stamper = new PdfStamper(reader, memoryStream);
		}

		/// <summary>
		/// METODO 1: ricerca di un acrofield generico per name, 
		/// l’oggetto ritornato deve indicare il tipo di acrofield(checkbox, textbox, signaturefield, radiobutton).
		/// </summary>
		/// <param name="fieldName">Field name</param>
		/// <returns>Field name</returns>
		public int GetAcrofieldType(string fieldName)
		{
			int type;                                               //Type of field

			//Checking if argument is null
			if (fieldName == null)
				throw new ArgumentNullException(fieldName);

			//Getting fields
			AcroFields form = reader.AcroFields;

			//Checking if document has no fields
			if (form.Fields.Count == 0)
				throw new DocumentHasNoFieldsException(filename);

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

			//If field not found, throw an exception
			throw new FieldNotFoundException(fieldName);
		}

		/// <summary>
		/// METODO 1.1 (aggiuntivo): Ritorna in formato "human readable" il tipo di field passato per parametro
		/// l’oggetto ritornato deve indicare il tipo di acrofield(checkbox, textbox, signaturefield, radiobutton)
		/// </summary>
		/// <param name="num">int Field type</param>
		/// <returns>string	Field type (human readable)</returns>
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

		/// <summary>
		///	METODO 2: Flaggare un acrofield di tipo checkbox
		/// Looking for a checkbox and checking it
		/// </summary>
		/// <param name="fieldName">string Name of the checkbox to check</param>
		/// <returns>bool Operation result</returns>
		public void FlagCheckbox(string fieldName)
		{
			//Checking if argument is null
			if (fieldName == null)
				throw new ArgumentNullException(fieldName);

			//Getting forms
			AcroFields form = stamper.AcroFields;

			//Checking if document has no fields
			if (form.Fields.Count == 0)
				throw new DocumentHasNoFieldsException(filename);

			//Looking for a checkbox with the given name
			var result = form.Fields.Where(kvp => form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_CHECKBOX && form.GetTranslatedFieldName(kvp.Key).Equals(fieldName))?.FirstOrDefault();

			if (result.Value.Key == null || result.Value.Value == null)
				throw new FieldNotFoundException(fieldName, AcroFields.FIELD_TYPE_CHECKBOX);

			string name = form.GetTranslatedFieldName(result.Value.Key);
			string[] values = form.GetAppearanceStates(name);

			//If the box isn't checked, we check it
			if (form.GetField(result.Value.Key).Equals(values[0]) || form.GetField(result.Value.Key).Length == 0)
			{
				//Changing state and returning true (in case of error it returns false)
				form.SetField(name, values[1]);
			}
		}

		/// <summary>
		/// METODO 3: Sostituire un acrofield di tipo signature con un acrofield di tipo checkbox
		/// Locking for a checkbox and checking it
		/// </summary>
		/// <param name="fieldName">string Name of the signaturefield to substitute</param>
		public void SubstituteSignature(string fieldName)
		{
			bool found = false;                                                 //Flag indicating if an unchecked checkbox has been found
			string key = null;													//Field's key
			IList<FieldPosition> positions = null;                              //Field's positions						
			string name;

			//Checking if argument is null
			if (fieldName == null)
				throw new ArgumentNullException(fieldName);

			//Getting forms
			AcroFields form = stamper.AcroFields;

			//Checking if document has no fields
			if (form.Fields.Count == 0)
				throw new DocumentHasNoFieldsException(filename);

			ArrayList arr = new ArrayList();

			//Analyzing every item
			foreach (KeyValuePair<string, AcroFields.Item> kvp in form.Fields)
			{
				arr.Add(kvp.Key);

				//Checking if the form is checkbox
				if (form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_SIGNATURE && (name = form.GetTranslatedFieldName(kvp.Key)).Equals(fieldName))
				{
					//Getting field's key
					key = kvp.Key;
					//Getting field's position(s)
					positions = form.GetFieldPositions(name);

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
			}

			if (!found)
				throw new FieldNotFoundException(fieldName, AcroFields.FIELD_TYPE_SIGNATURE);
		}

		/// <summary>
		/// METODO 4: Selezionare un acrofield di tipo radiobutton
		/// Selecting a radiobutton acrofield
		/// </summary>
		/// <param name="fieldName">string Name of the radiobutton to select</param>
		/// <param name="valueToSelect">string Value to select</param>
		public void SelectRadiobutton(string fieldName, string valueToSelect)
		{
			bool found = false;                                                 //Flag indicating if an unchecked checkbox has been found
			string name;

			//Checking if argument is null
			if (fieldName == null)
				throw new ArgumentNullException("fieldName");
			if (valueToSelect == null)
				throw new ArgumentNullException("valueToSelect");

			//Getting forms
			AcroFields form = stamper.AcroFields;

			//Checking if document has no fields
			if (form.Fields.Count == 0)
				throw new DocumentHasNoFieldsException(filename);

			//Analyzing every item
			foreach (KeyValuePair<string, AcroFields.Item> kvp in form.Fields)
			{
				//Cheking if textfield
				if (form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_RADIOBUTTON)
				{
					if((name = form.GetTranslatedFieldName(kvp.Key)).Equals(fieldName))
					{
						//Getting radiobutton values
						string[] values = form.GetAppearanceStates(kvp.Key);

						//Checking if value to select exists
						if (!values.Contains(valueToSelect))
							throw new RadiobuttonValueNotFoundException(fieldName, valueToSelect);

						//Setting the value
						found = form.SetField(form.GetTranslatedFieldName(kvp.Key), valueToSelect);
					}
				}
			}

			if (!found)
				throw new FieldNotFoundException(fieldName, AcroFields.FIELD_TYPE_RADIOBUTTON);
		}

		/// <summary>
		///  METODO 5: Inserire un testo in un acrofield ti tipo testo
		///  Inserting the given text into a textfield
		/// </summary>
		/// <param name="fieldName">string Name of the textfield to modify</param>
		/// <param name="text">string Text to insert</param>
		public void InsertTextInField(string fieldName, string text)
		{
			bool found = false;                                                 //Flag indicating if an unchecked checkbox has been found

			//Checking if argument is null
			if (fieldName == null)
				throw new ArgumentNullException("fieldName");
			if (text == null)
				throw new ArgumentNullException("valueToSelect");

			//Getting forms
			AcroFields form = stamper.AcroFields;

			//Checking if document has no fields
			if (form.Fields.Count == 0)
				throw new DocumentHasNoFieldsException(filename);

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

			if (!found)
				throw new FieldNotFoundException(fieldName, AcroFields.FIELD_TYPE_RADIOBUTTON);
		}

		/// <summary>
		///  METODO 6: Ottenimento del pdf elaborato
		///  Saving the working copy on file
		/// </summary>
		public void Save()
		{
			//Closing stamper
			stamper.Dispose();
			
			//Flattening Document
			//FlatteningDocument();

			//Saving data
			var data = memoryStream.ToArray();

			using (var fs = new FileStream(filename_out, FileMode.Create, FileAccess.Write))
			{
				fs.Write(data, 0, data.Length);
			}

			////Saving data
			//var data = memoryStream.ToArray();

			////Saving on file
			//using (PdfReader dataReader = new PdfReader(data))
			//using (PdfStamper filestamper = new PdfStamper(dataReader, new FileStream(Filename_out, FileMode.Create)))
			//{
			//	//Setting Flattening
			//	filestamper.FormFlattening = true;
			//}

			//Notifying that stamper has been disposed
			stamperDisposed = true;
		}

		/// <summary>
		///  METODO 6.1: "Appiattimento" del pdf prima della scrittura su file
		///  Flattening of the pdf before writing to file
		/// </summary>
		public void FlatteningDocument()
		{
			stamper.FormFlattening = true;
		}
	}
}