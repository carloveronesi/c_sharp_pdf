using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using static iTextSharp.text.pdf.AcroFields;
using System.Linq;

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
			stamper = new PdfStamper(reader, memoryStream)
			{
				FormFlattening = true
			};
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
				throw new ArgumentNullException();

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
	}
}
