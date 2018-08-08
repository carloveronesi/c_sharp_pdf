using System;
using System.IO;
using IDSign.PdfUtility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElaborazionePdf.UnitTests
{
	[TestClass]
	public class PdfUtilityTests
	{
		private const string FILE_WITH_CHECKBOX = @"TestFiles\Richiesta di adesione e Condizioni relative all'uso della firma elettronica avanzata_checkbox.pdf";
		private const string FILE_WITH_SIGNATUREFIELD = @"TestFiles\Richiesta di adesione e Condizioni relative all'uso della firma elettronica avanzata_signaturefield.pdf";
		private const string FILE_WITH_RADIOBUTTON = @"TestFiles\test_radiobutton.pdf";
		private const string FILE_WITH_NO_FIELDS = @"TestFiles\No_fields.pdf";
		
		/*!
		 Constructor tests
		 */

		[TestMethod]
		public void Constructor_FileExists_NoExceptions()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null)) { }
		}

		[TestMethod]
		[ExpectedException(typeof(IOException))]
		public void Constructor_FileDoesntExist_ThrowsException()
		{
			//Arrange / Act
			using (PdfUtility doc = new PdfUtility(@"TestFiles\filenonesistente.pdf", null)){ }
		}

		/*!
		 Method 1 tests
		 */

		[TestMethod]
		[ExpectedException(typeof(DocumentHasNoFieldsException))]
		public void GetAcrofieldType_DocumentHasNoFields_DocumentHasNoFieldsException()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_NO_FIELDS, null))
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
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				var type = doc.GetAcrofieldType(null);
			}
		}

		[TestMethod]
		public void GetAcrofieldType_FieldExists_ReturnsFieldType()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
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
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				var type = doc.GetAcrofieldType("Nomi");
			}
		}

		/// <summary>
		/// Method 2 tests
		/// </summary>


		[TestMethod]
		[ExpectedException(typeof(DocumentHasNoFieldsException))]
		public void FlagCheckbox_DocumentHasNoFields_DocumentHasNoFieldsException()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_NO_FIELDS, null))
			{
				//Act
				var result = doc.FlagCheckbox("Pluto");
			}
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void FlagCheckbox_ArgumentNull_ArgumentNullException()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				var type = doc.FlagCheckbox(null);
			}
		}

		[TestMethod]
		public void FlagCheckbox_CheckboxExists_ReturnsTrue()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				var result = doc.FlagCheckbox("CheckBox1");

				//Assert
				Assert.IsTrue(result);
			}
		}
		
		[TestMethod]
		public void FlagCheckbox_TwoCheckableCheckboxes_ReturnsTrue()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				//Flagging first checkbox
				var result1 = doc.FlagCheckbox("CheckBox1");
				//Flagging second
				var result2 = doc.FlagCheckbox("CheckBox2");

				//Assert
				Assert.IsTrue(result1 && result2);
			}
		}
		
		[TestMethod]
		[ExpectedException(typeof(FieldNotFoundException))]
		public void FlagCheckbox_CheckingTwice_FieldNotFoundException()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				//Flagging first 
				var result1 = doc.FlagCheckbox("CheckBox1");
				//Flagging second
				var result2 = doc.FlagCheckbox("CheckBox1");
			}
		}
		
		[TestMethod]
		[ExpectedException(typeof(FieldNotFoundException))]
		public void FlagCheckbox_CheckboxDoesntExist_FieldNotFoundException()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_SIGNATUREFIELD, null))
			{
				//Act
				var result = doc.FlagCheckbox("Pluto");
			}
		}

		/// <summary>
		///  Method 3 tests
		/// </summary>

		[TestMethod]
		[ExpectedException(typeof(DocumentHasNoFieldsException))]
		public void SubstituteSignature_DocumentHasNoFields_DocumentHasNoFieldsException()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_NO_FIELDS, null))
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
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				doc.SubstituteSignature(null);
			}
		}

		[TestMethod]
		public void SubstituteSignature_SignatureExists()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_SIGNATUREFIELD, null))
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
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				doc.SubstituteSignature("Pluto");
			}
		}

		
		[TestMethod]
		public void SubstituteSignature_TwoSignatureFields()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_SIGNATUREFIELD, null))
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
			using (PdfUtility doc = new PdfUtility(FILE_WITH_SIGNATUREFIELD, null))
			{
				//Act
				//Flagging first checkbox
				doc.SubstituteSignature("Signature1");
				//Flagging second
				doc.SubstituteSignature("Signature1");
			}
		}



		/// <summary>
		/// Method 4 tests
		/// </summary>
		/// 

		[TestMethod]
		[ExpectedException(typeof(DocumentHasNoFieldsException))]
		public void SelectRadiobutton_DocumentHasNoFields_DocumentHasNoFieldsException()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_NO_FIELDS, null))
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
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
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
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				doc.SelectRadiobutton("Signature1", null);
			}
		}

		[TestMethod]
		public void SelectRadiobutton_RadiobuttonExists()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_RADIOBUTTON, null))
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
			using (PdfUtility doc = new PdfUtility(FILE_WITH_RADIOBUTTON, null))
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
			using (PdfUtility doc = new PdfUtility(FILE_WITH_RADIOBUTTON, null))
			{
				//Act
				doc.SelectRadiobutton("language_gc", "Pluto");
			}
		}

		[TestMethod]
		public void SelectRadiobutton_SelectingTwiceRadiobutton()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_RADIOBUTTON, null))
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
			using (PdfUtility doc = new PdfUtility(FILE_WITH_RADIOBUTTON, null))
			{
				//Act
				doc.SelectRadiobutton("language_gc", "English");
				doc.SelectRadiobutton("language_gc", "Spanish");
			}
		}

		/// <summary>
		/// Method 5 tests
		/// </summary>

		[TestMethod]
		[ExpectedException(typeof(DocumentHasNoFieldsException))]
		public void InsertTextInField_DocumentHasNoFields_DocumentHasNoFieldsException()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_NO_FIELDS, null))
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
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
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
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				doc.InsertTextInField("Nome", null);
			}
		}

		[TestMethod]
		public void InsertTextInField_FieldExists()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
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
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				doc.InsertTextInField("Nomi", "Pippo");
			}
		}
		
		[TestMethod]
		public void InsertTextInField_InsertingTextTwice()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act

				//Inserting first time
				doc.InsertTextInField("Nome", "Pippo");
				//Inserting second time
				doc.InsertTextInField("Nome", "Pippo2");
			}
		}
		
		/// <summary>
		///  Method 6 tests
		/// </summary>

		[TestMethod]
		public void Save_DocumentUntouched()
		{
			//Arrange

			//Generating output file name (filename + _modified.pdf)
			var filename_out = FILE_WITH_CHECKBOX.Substring(0, FILE_WITH_CHECKBOX.Length - 4) + "_modified.pdf";
			//Checking if output file exists, in case we delete it
			if (File.Exists(filename_out))
			{
				File.Delete(filename_out);
			}

			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				doc.Save();
			}
		}

		[TestMethod]
		public void Save_DocumentTouched()
		{
			//Arrange

			//Generating output file name (filename + _modified.pdf)
			var filename_out = FILE_WITH_CHECKBOX.Substring(0, FILE_WITH_CHECKBOX.Length - 4) + "_modified.pdf";
			//Checking if output file exists, in case we delete it
			if (File.Exists(filename_out))
			{
				File.Delete(filename_out);
			}

			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Modifying file
				doc.FlagCheckbox("CheckBox1");
				doc.InsertTextInField("Nome", "Pippo");

				//Act
				doc.Save();
			}
		}
	
	}
}
