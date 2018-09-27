using System;
using System.IO;
using IDSign.PdfUtility;

namespace ElaborazionePdf
{
	public class Test
	{
		static void Main(string[] args)
		{
			int option = 0;
			string name;
			string value;

			byte [] file = File.ReadAllBytes(@"C:\Users\c.veronesi\source\repos\ElaborazionePdf\ElaborazionePdf.UnitTests\TestFiles\Richiesta di adesione e Condizioni relative all'uso della firma elettronica avanzata_checkbox.pdf"); ;

			using (PdfUtility p = new PdfUtility(file, @"C:\Users\c.veronesi\source\repos\ElaborazionePdf\ElaborazionePdf.UnitTests\TestFiles\Richiesta di adesione e Condizioni relative all'uso della firma elettronica avanzata_checkbox.pdf", PrintLog))
			{
				do
				{
					Console.WriteLine("\nMENU\n\n1. Metodo: ricerca di un acrofield generico per name, l’oggetto ritornato deve indicare il tipo di acrofield(checkbox, textbox, signaturefield, radiobutton)\n2. Metodo: per flaggare un acrofield di tipo checkbox\n3. Metodo: per sostituire un acrofield di tipo signature con un acrofield di tipo checkbox\n4. Metodo: per selezionare un acrofield di tipo radiobutton\n5. Metodo: per inserire un testo in un acrofield di tipo testo\n6. Metodo: per ottenere il pdf elaborato\n7. Appiattisci pdf\n8. Esci\n\nInserisci la tua scelta:");
					try
					{
						option = Int32.Parse(Console.ReadLine());
					}
					catch (FormatException)
					{
						Console.WriteLine("\nWrong command");
					}

					try
					{
						switch (option)
						{
							case 1:
								Console.WriteLine("\nInsert field name: ");
								name = Console.ReadLine();

								int fieldType = p.GetAcrofieldType(name);
								Console.WriteLine("Found type: " + fieldType + " (" + PdfUtility.GetFormType(fieldType) + ")");

								break;
							case 2:
								Console.WriteLine("\nInsert checkbox name: ");
								name = Console.ReadLine();

								p.FlagCheckbox(name);
								Console.WriteLine("Checked successfully!");
								break;
							case 3:
								Console.WriteLine("\nInsert signaturefield name: ");
								name = Console.ReadLine();

								p.SubstituteSignature(name);
								Console.WriteLine("Substituted successfully!");
								break;
							case 4:
								Console.WriteLine("\nInsert radiobutton name: ");
								name = Console.ReadLine();
								Console.WriteLine("\nInsert value to select: ");
								value = Console.ReadLine();

								p.SelectRadiobutton(name, value);
								Console.WriteLine("Renamed successfully!");
								break;
							case 5:
								Console.WriteLine("\nInsert field name: ");
								name = Console.ReadLine();
								Console.WriteLine("\nInsert text: ");
								value = Console.ReadLine();

								p.InsertTextInField(name, value);
								Console.WriteLine("Text inserted successfully!");
								break;
							case 6:
								p.Save();
								Console.WriteLine("File \"" + p.Filename_out + "\" saved successfully!");
								break;
							case 7:
								p.FlatteningDocument();
								break;
							case 8:
								break;
						}
					}
					catch (DocumentHasNoFieldsException)
					{
						Console.WriteLine("ERROR: Document has no fields");
					}
					catch (FieldNotFoundException)
					{
						Console.WriteLine("ERROR: Field not found");
					}
					catch (RadiobuttonValueNotFoundException)
					{
						Console.WriteLine("ERROR: The inserted value was not found in this radiobutton field");
					}
				}
				while (option < 6 || option == 7 || option > 8);
			}
			CloseProgram();
		}

		/// <summary>
		/// Caller's Logger function
		/// </summary>
		/// <param name="x">Message to print</param>
		static void PrintLog(string x)
		{
			Console.WriteLine("----------Logger message----------");
			Console.WriteLine(x);
			Console.WriteLine("----------------------------------");
		}

		/// <summary>
		///  Press any key to exit...
		/// </summary>
		private static void CloseProgram()
		{
			Console.WriteLine("\n\nPress any key to exit...");
			Console.ReadLine();
		}
	}
}
