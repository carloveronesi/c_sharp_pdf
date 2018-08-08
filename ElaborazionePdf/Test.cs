using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using IDSign.PdfUtility;
using iTextSharp.text;
using iTextSharp.text.pdf;
using static iTextSharp.text.pdf.AcroFields;

namespace ElaborazionePdf
{
	public class Test
	{
		static void Main(string[] args)
		{
			int option = 0;
			string name;
			string value;

			try
			{
				using (PdfUtility p = new PdfUtility(@"C:\Users\c.veronesi\source\repos\ElaborazionePdf\ElaborazionePdf.UnitTests\TestFiles\Richiesta di adesione e Condizioni relative all'uso della firma elettronica avanzata_checkbox.pdf", PrintLog))
				{
					do
					{
						Console.WriteLine("\nMENU\n\n1. Metodo: ricerca di un acrofield generico per name, l’oggetto ritornato deve indicare il tipo di acrofield(checkbox, textbox, signaturefield, radiobutton)\n2. Metodo: per flaggare un acrofield di tipo checkbox\n3. Metodo: per sostituire un acrofield di tipo signature con un acrofield di tipo checkbox\n4. Metodo: per selezionare un acrofield di tipo radiobutton\n5. Metodo: per inserire un testo in un acrofield di tipo testo\n6. Metodo: per ottenere il pdf elaborato\n7. Esci\n\nInserisci la tua scelta:");
						try
						{
							option = Int32.Parse(Console.ReadLine());
						}
						catch(FormatException)
						{}

						switch (option)
						{
							case 1:
								Console.WriteLine("\nInsert field name: ");
								name = Console.ReadLine();

								try
								{
									int fieldType = p.GetAcrofieldType(name);
									Console.WriteLine("Found type: " + fieldType + " (" + PdfUtility.GetFormType(fieldType) + ")");
								}
								catch (DocumentHasNoFieldsException)
								{
									Console.WriteLine("ERROR: Document has no fields");
								}
								catch (FieldNotFoundException)
								{
									Console.WriteLine("ERROR: Field not found");
								}

								break;
							case 2:
								Console.WriteLine("\nInsert checkbox name: ");
								name = Console.ReadLine();

								try
								{
									p.FlagCheckbox(name);
								}
								catch (DocumentHasNoFieldsException)
								{
									Console.WriteLine("ERROR: Document has no fields");
								}
								catch (FieldNotFoundException)
								{
									Console.WriteLine("ERROR: Field not found");
								}

								break;
							case 3:
								Console.WriteLine("\nInsert signaturefield name: ");
								name = Console.ReadLine();

								try
								{
									p.SubstituteSignature(name);
								}
								catch (DocumentHasNoFieldsException)
								{
									Console.WriteLine("ERROR: Document has no fields");
								}
								catch (FieldNotFoundException)
								{
									Console.WriteLine("ERROR: Field not found");
								}

								break;
							case 4:
								Console.WriteLine("\nInsert radiobutton name: ");
								name = Console.ReadLine();
								Console.WriteLine("\nInsert value to select: ");
								value = Console.ReadLine();
								/////////////////////RadiobuttonValueNotFoundException
								try
								{
									p.SelectRadiobutton(name, value);
								break;
							case 5:
								Console.WriteLine("\nInsert field name: ");
								name = Console.ReadLine();
								Console.WriteLine("\nInsert text: ");
								value = Console.ReadLine();
								p.InsertTextInField(name, value);
								break;
							case 6:
								p.Save();
								Console.WriteLine("File \"" + p.Filename_out + "\" saved.");
								break;
							case 7:
								break;
							default:
								Console.WriteLine("Wrong command");
								break;
						}
					}
					while (option < 6 || option > 7);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			CloseProgram();
		}

		/*!
		 Caller's Logger function
		 */
		static void PrintLog(string x) {
			Console.WriteLine("----------Logger message----------");
			Console.WriteLine(x);
			Console.WriteLine("----------------------------------");
		}

		/*!
		 Press any key to exit...
		 */
		private static void CloseProgram()
		{
			Console.WriteLine("\n\nPress any key to exit...");
			Console.ReadLine();
		}
	}
}
