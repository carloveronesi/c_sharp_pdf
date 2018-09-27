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
		public PdfUtility(byte[] data, string filename, LoggerFunction funct)
		{
			//Delegating logger
			delegateFunction = funct;
			//Saving filename
			this.filename = filename;
			//Generating output file name (filename + _modified.pdf)
			Filename_out = filename.Substring(0, filename.Length - 4) + "_modified.pdf";

			memoryStream = new MemoryStream(data);
			reader = new PdfReader(data);
			stamper = new PdfStamper(reader, memoryStream);
			//Loading file
			//LoadFile();
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
			//Checking if argument is null
			if (fieldName == null) { 
				throw new ArgumentNullException(fieldName);
			}

			//Getting fields
			AcroFields form = reader.AcroFields;

			//Checking if document has no fields
			if (form.Fields.Count == 0)
			{
				throw new DocumentHasNoFieldsException(filename);
			}

			//Looking for a checkbox/radiobutton/signature/text withe the given name
			var result = form.Fields
				.Where(kvp =>
					form.GetTranslatedFieldName(kvp.Key).Equals(fieldName) &&
					(
					form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_CHECKBOX ||
					form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_RADIOBUTTON ||
					form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_SIGNATURE ||
					form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_TEXT
					)
				)
				.Select(kvp =>  form.GetFieldType(kvp.Key))?.FirstOrDefault();

			//If field not found (default is 0), throw an exception
			if (result == 0)
			{
				throw new FieldNotFoundException(fieldName);
			}
			
			//Returning the field's type
			return (int)result;
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
			var result = form.Fields
				.Where(kvp => 
					form.GetTranslatedFieldName(kvp.Key).Equals(fieldName) &&
					form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_CHECKBOX
					)
				.Select(kvp => 
					new {
						kvp.Key,
						Name = form.GetTranslatedFieldName(kvp.Key),
						Values = form.GetAppearanceStates(form.GetTranslatedFieldName(kvp.Key))
					})
				?.FirstOrDefault();

			//Checking if the query had results
			if (result == null)
				throw new FieldNotFoundException(fieldName, AcroFields.FIELD_TYPE_CHECKBOX);

			//If the box isn't checked, we check it
			if (form.GetField(result.Key).Equals(result.Values[0]) || form.GetField(result.Key).Length == 0)
			{
				//Changing state and returning true (in case of error it returns false)
				form.SetField(result.Name, result.Values[1]);
			}
		}

		/// <summary>
		/// METODO 3: Sostituire un acrofield di tipo signature con un acrofield di tipo checkbox
		/// Locking for a checkbox and checking it
		/// </summary>
		/// <param name="fieldName">string Name of the signaturefield to substitute</param>
		public void SubstituteSignature(string fieldName)
		{
			//Checking if argument is null
			if (fieldName == null)
			{
				throw new ArgumentNullException(fieldName);
			}

			//Getting fields
			AcroFields form = reader.AcroFields;

			//Checking if document has no fields
			if (form.Fields.Count == 0)
			{
				throw new DocumentHasNoFieldsException(filename);
			}

			//Looking for a signatureBox with the given name
			var result = form.Fields
				.Where(kvp => 
					form.GetTranslatedFieldName(kvp.Key).Equals(fieldName) &&
					form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_SIGNATURE
				)
				.Select(kvp => new { kvp.Key, Position = form.GetFieldPositions(kvp.Key) })
				?.FirstOrDefault();

			//Checking if the query had results
			if (result == null)
			{
				throw new FieldNotFoundException(fieldName, AcroFields.FIELD_TYPE_SIGNATURE);
			}

			//Removing field
			form.RemoveField(result.Key);

			//Creating new checkbox with signaturefield's coordinates
			//Note: We're replacing the first occurrence
			RadioCheckField checkbox = new RadioCheckField(stamper.Writer, result.Position[0].position, "i_was_a_signature_field", "Yes")
			{
				//Setting look
				CheckType = RadioCheckField.TYPE_CHECK,
				Checked = true,
				BorderWidth = BaseField.BORDER_WIDTH_THIN,
				BorderColor = BaseColor.BLACK,
				BackgroundColor = BaseColor.WHITE
			};

			//Adding checbox in signaturefield's page
			stamper.AddAnnotation(checkbox.CheckField, result.Position[0].page);
		}

		/// <summary>
		/// METODO 4: Selezionare un acrofield di tipo radiobutton
		/// Selecting a radiobutton acrofield
		/// </summary>
		/// <param name="fieldName">string Name of the radiobutton to select</param>
		/// <param name="valueToSelect">string Value to select</param>
		public void SelectRadiobutton(string fieldName, string valueToSelect)
		{
			//Checking if argument is null
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}

			if (valueToSelect == null)
			{
				throw new ArgumentNullException("valueToSelect");
			}

			//Getting forms
			AcroFields form = stamper.AcroFields;

			//Checking if document has no fields
			if (form.Fields.Count == 0)
			{
				throw new DocumentHasNoFieldsException(filename);
			}

			//Looking for a radiobutton with the given name
			var result = form.Fields
				.Where(kvp => 
					form.GetTranslatedFieldName(kvp.Key).Equals(fieldName) &&
					form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_RADIOBUTTON)
				.Select(kvp => new { kvp.Key, States = form.GetAppearanceStates(kvp.Key) })
				?.FirstOrDefault();

			//Checking if the query had results
			if (result == null)
			{
				throw new FieldNotFoundException(fieldName, AcroFields.FIELD_TYPE_RADIOBUTTON);
			}

			//Checking if value to select exists
			if (!result.States.Contains(valueToSelect))
			{
				throw new RadiobuttonValueNotFoundException(fieldName, valueToSelect);
			}

			//Setting the value
			form.SetField(form.GetTranslatedFieldName(result.Key), valueToSelect);
		}

		/// <summary>
		///  METODO 5: Inserire un testo in un acrofield ti tipo testo
		///  Inserting the given text into a textfield
		/// </summary>
		/// <param name="fieldName">string Name of the textfield to modify</param>
		/// <param name="text">string Text to insert</param>
		public void InsertTextInField(string fieldName, string text)
		{
			//Checking if argument is null
			if (fieldName == null)
			{
				throw new ArgumentNullException("fieldName");
			}
			if (text == null)
			{
				throw new ArgumentNullException("valueToSelect");
			}

			//Getting forms
			AcroFields form = stamper.AcroFields;

			//Checking if document has no fields
			if (form.Fields.Count == 0)
			{
				throw new DocumentHasNoFieldsException(filename);
			}

			//Looking for a text-field with the given name
			var result = form.Fields
				.Where(kvp =>
					form.GetTranslatedFieldName(kvp.Key).Equals(fieldName) &&
					form.GetFieldType(kvp.Key) == AcroFields.FIELD_TYPE_TEXT
				)
				.Select(kvp => form.GetTranslatedFieldName(kvp.Key))
				?.FirstOrDefault();

			//Checking if the query had results
			if (result == null)
			{
				throw new FieldNotFoundException(fieldName, AcroFields.FIELD_TYPE_RADIOBUTTON);
			}

			//Setting the given text
			form.SetField(fieldName, text);
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