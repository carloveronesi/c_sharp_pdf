using System;
using System.IO;
using System.Linq;
using IDSign.PdfUtility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElaborazionePdf.UnitTests
{
	[TestClass]
	public class PdfUtilityTests
	{
		private const string FILE_NO_PDF = @"TestFiles\Immagine.jpg";
		private const string FILE_WITH_CHECKBOX = @"TestFiles\Richiesta di adesione e Condizioni relative all'uso della firma elettronica avanzata_checkbox.pdf";
		private const string FILE_WITH_SIGNATUREFIELD = @"TestFiles\Richiesta di adesione e Condizioni relative all'uso della firma elettronica avanzata_signaturefield.pdf";
		private const string FILE_WITH_RADIOBUTTON = @"TestFiles\test_radiobutton.pdf";
		private const string FILE_WITH_NO_FIELDS = @"TestFiles\No_fields.pdf";

		#region Constructor tests
		[TestMethod]
		public void Constructor_FileExists_NoExceptions()
		{
			byte[] file = File.ReadAllBytes(FILE_WITH_NO_FIELDS);
			PdfUtility doc = new PdfUtility(file, null);

			//Disposing the element
			doc.Dispose();
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException))]
		public void Constructor_FileWithWrongFormat_FileNotFoundException()
		{
			byte[] file = File.ReadAllBytes(FILE_NO_PDF);
			PdfUtility doc = new PdfUtility(file, null);

			//Disposing the element
			doc.Dispose();
		}

		[TestMethod]
		[ExpectedException(typeof(IOException))]
		public void Constructor_FileDoesntExist_ThrowsException()
		{
			byte[] file = new byte[0];
			using (PdfUtility doc = new PdfUtility(file, null)){ }
		}
		#endregion

		#region Method 1 tests
		[TestMethod]
		[ExpectedException(typeof(DocumentHasNoFieldsException))]
		public void GetAcrofieldType_DocumentHasNoFields_DocumentHasNoFieldsException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_NO_FIELDS);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				var type = doc.GetAcrofieldType("Nomi");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetAcrofieldType_ArgumentNull_ArgumentNullException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				var type = doc.GetAcrofieldType(null);
			}
		}

		[TestMethod]
		public void GetAcrofieldType_FieldExists_ReturnsFieldType()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				var type = doc.GetAcrofieldType("Nome");
				System.Diagnostics.Debug.WriteLine("Looking for acrofield named \"Nome\", type: " + type + " (" + PdfUtility.GetFormType(type) + ")");

				//Assert
				Assert.IsTrue(type > -1);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(FieldNotFoundException))]
		public void GetAcrofieldType_FieldDoesntExist_FieldNotFoundException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				var type = doc.GetAcrofieldType("Nomi");
			}
		}
		#endregion

		#region Method 2 tests
		[TestMethod]
		[ExpectedException(typeof(DocumentHasNoFieldsException))]
		public void FlagCheckbox_DocumentHasNoFields_DocumentHasNoFieldsException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_NO_FIELDS);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.FlagCheckbox("Pluto");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FlagCheckbox_ArgumentNull_ArgumentNullException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.FlagCheckbox(null);
			}
		}

		[TestMethod]
		public void FlagCheckbox_CheckboxExists()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.FlagCheckbox("CheckBox1");
			}
		}
		
		[TestMethod]
		public void FlagCheckbox_TwoCheckableCheckboxes()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				//Flagging first checkbox
				doc.FlagCheckbox("CheckBox1");
				//Flagging second
				doc.FlagCheckbox("CheckBox2");
			}
		}
		
		[TestMethod]
		public void FlagCheckbox_CheckingTwice()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Flagging first 
				doc.FlagCheckbox("CheckBox1");
				//Flagging second
				doc.FlagCheckbox("CheckBox1");
			}
		}
		
		[TestMethod]
		[ExpectedException(typeof(FieldNotFoundException))]
		public void FlagCheckbox_CheckboxDoesntExist_FieldNotFoundException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.FlagCheckbox("Pluto");
			}
		}
		#endregion

		#region Method 3 tests
		[TestMethod]
		[ExpectedException(typeof(DocumentHasNoFieldsException))]
		public void SubstituteSignature_DocumentHasNoFields_DocumentHasNoFieldsException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_NO_FIELDS);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.SubstituteSignature("Signature1");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SubstituteSignature_ArgumentNull_ArgumentNullException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.SubstituteSignature(null);
			}
		}

		[TestMethod]
		public void SubstituteSignature_SignatureExists()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_SIGNATUREFIELD);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.SubstituteSignature("Signature1");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(FieldNotFoundException))]
		public void SubstituteSignature_SignatureDoesntExist_FieldNotFoundException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.SubstituteSignature("Pluto");
			}
		}

		[TestMethod]
		public void SubstituteSignature_TwoSignatureFields()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_SIGNATUREFIELD);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				//Flagging first checkbox
				doc.SubstituteSignature("Signature1");
				//Flagging second
				doc.SubstituteSignature("Signature2");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(FieldNotFoundException))]
		public void SubstituteSignature_SubstituteTwice_FieldNotFoundException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_SIGNATUREFIELD);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				//Flagging first checkbox
				doc.SubstituteSignature("Signature1");
				//Flagging second
				doc.SubstituteSignature("Signature1");
			}
		}
		#endregion

		#region Method 4 tests
		[TestMethod]
		[ExpectedException(typeof(DocumentHasNoFieldsException))]
		public void SelectRadiobutton_DocumentHasNoFields_DocumentHasNoFieldsException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_NO_FIELDS);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.SelectRadiobutton("Signature1", "English");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SelectRadiobutton_FieldArgumentNull_ArgumentNullException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.SelectRadiobutton(null, "English");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SelectRadiobutton_ValueArgumentNull_ArgumentNullException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.SelectRadiobutton("Signature1", null);
			}
		}

		[TestMethod]
		public void SelectRadiobutton_RadiobuttonExists()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_RADIOBUTTON);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.SelectRadiobutton("language_gc", "English");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(FieldNotFoundException))]
		public void SelectRadiobutton_RadiobuttonDoesntExist_FieldNotFoundException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_RADIOBUTTON);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.SelectRadiobutton("Pluto", "English");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(RadiobuttonValueNotFoundException))]
		public void SelectRadiobutton_ValueToSelectDoesntExist_FieldNotFoundException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_RADIOBUTTON);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.SelectRadiobutton("language_gc", "Pluto");
			}
		}

		[TestMethod]
		public void SelectRadiobutton_SelectingTwiceRadiobutton()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_RADIOBUTTON);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.SelectRadiobutton("language_gc", "English");
				doc.SelectRadiobutton("language_gc", "English");
			}
		}

		[TestMethod]
		public void SelectRadiobutton_SelectingTwiceRadiobuttonAssigningDifferentValues()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_RADIOBUTTON);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.SelectRadiobutton("language_gc", "English");
				doc.SelectRadiobutton("language_gc", "Spanish");
			}
		}
		#endregion


		#region Method 5 tests
		[TestMethod]
		[ExpectedException(typeof(DocumentHasNoFieldsException))]
		public void InsertTextInField_DocumentHasNoFields_DocumentHasNoFieldsException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_NO_FIELDS);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.InsertTextInField("Nome", "Pippo");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InsertTextInField_FieldArgumentNull_ArgumentNullException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.InsertTextInField(null, "Pippo");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void InsertTextInField_TextArgumentNull_ArgumentNullException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.InsertTextInField("Nome", null);
			}
		}

		[TestMethod]
		public void InsertTextInField_FieldExists()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.InsertTextInField("Nome", "Pippo");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(FieldNotFoundException))]
		public void InsertTextInField_FieldDoesntExist_FieldNotFoundException()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				doc.InsertTextInField("Nomi", "Pippo");
			}
		}

		[TestMethod]
		public void InsertTextInField_InsertingTextTwice()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);
			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act

				//Inserting first time
				doc.InsertTextInField("Nome", "Pippo");
				//Inserting second time
				doc.InsertTextInField("Nome", "Pippo2");
			}
		}
		#endregion

		#region Method 6 tests
		[TestMethod]
		public void Save_DocumentUntouched()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);

			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Act
				var file_out = doc.Save();

				using (var fs = new FileStream(@"TestFiles\prova.pdf", FileMode.Create, FileAccess.Write))
				{
					fs.Write(file_out, 0, file_out.Length);
				}

				//checking if the given file isn't null
				Assert.IsTrue(file_out != null);
			}
		}

		[TestMethod]
		public void Save_DocumentTouched()
		{
			//Arrange
			byte[] file = File.ReadAllBytes(FILE_WITH_CHECKBOX);

			using (PdfUtility doc = new PdfUtility(file, null))
			{
				//Modifying file
				doc.FlagCheckbox("CheckBox1");
				doc.InsertTextInField("Nome", "Pippo");

				//Act
				var file_out = doc.Save();

				//checking if the given file isn't null
				Assert.IsTrue(file_out != null);
			}
		}
		#endregion
	}
}
