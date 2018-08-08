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
		public void GetAcrofieldType_FieldDoesntExist_ReturnsMinusOne()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				var type = doc.GetAcrofieldType("Nomi");
			}
		}
		/*
		/// <summary>
		/// Method 2 tests
		/// </summary>

		[TestMethod]
		public void FlagCheckbox_CheckboxExists_ReturnsTrue()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				var result = doc.FlagCheckbox();

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
				var result1 = doc.FlagCheckbox();
				//Flagging second
				var result2 = doc.FlagCheckbox();

				//Assert
				Assert.IsTrue(result1 && result2);
			}
		}

		[TestMethod]
		public void FlagCheckbox_TwoCheckableCheckboxesButTryingToCheckThree_ReturnsFalse()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				//Flagging first checkbox
				var result1 = doc.FlagCheckbox();
				//Flagging second
				var result2 = doc.FlagCheckbox();
				//Flagging third
				var result3 = doc.FlagCheckbox();

				//Assert
				Assert.IsFalse(result1 && result2 && result3);
			}
		}

		[TestMethod]
		public void FlagCheckbox_CheckboxDoesntExist_ReturnsFalse()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_SIGNATUREFIELD, null))
			{
				//Act
				var result = doc.FlagCheckbox();

				//Assert
				Assert.IsFalse(result);
			}
		}

		/// <summary>
		///  Method 3 tests
		/// </summary>

		[TestMethod]
		public void SubstituteSignature_SignatureExists_ReturnsTrue()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_SIGNATUREFIELD, null))
			{
				//Act
				var result = doc.SubstituteSignature();

				//Assert
				Assert.IsTrue(result);
			}
		}

		[TestMethod]
		public void SubstituteSignature_TwoSignatureFields_ReturnsTrue()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_SIGNATUREFIELD, null))
			{
				//Act
				//Flagging first checkbox
				var result1 = doc.SubstituteSignature();
				//Flagging second
				var result2 = doc.SubstituteSignature();

				//Assert
				Assert.IsTrue(result1 && result2);
			}
		}

		[TestMethod]
		public void SubstituteSignature_TwoSignatureFieldsButTryingToSubstituteThree_ReturnsFalse()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_SIGNATUREFIELD, null))
			{
				//Act
				//Flagging first checkbox
				var result1 = doc.SubstituteSignature();
				//Flagging second
				var result2 = doc.SubstituteSignature();
				//Flagging third
				var result3 = doc.SubstituteSignature();

				//Assert
				Assert.IsFalse(result1 && result2 && result3);
			}
		}

		[TestMethod]
		public void SubstituteSignature_SignatureDoesntExist_ReturnsFalse()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				var result = doc.SubstituteSignature();

				//Assert
				Assert.IsFalse(result);
			}
		}


		/// <summary>
		/// Method 4 tests
		/// </summary>

		[TestMethod]
		public void SelectRadiobutton_RadiobuttonExists_ReturnsTrue()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_RADIOBUTTON, null))
			{
				//Act
				var result = doc.SelectRadiobutton();

				//Assert
				Assert.IsTrue(result);
			}
		}

		[TestMethod]
		public void SelectRadiobutton_RadiobuttonDoesntExist_ReturnsTrue()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				var result = doc.SelectRadiobutton();

				//Assert
				Assert.IsFalse(result);
			}
		}

		[TestMethod]
		public void SelectRadiobutton_SelectingTwiceRadiobutton_ReturnsTrue()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_RADIOBUTTON, null))
			{
				//Act
				var result = doc.SelectRadiobutton();
				var result2 = doc.SelectRadiobutton();

				//Assert
				Assert.IsTrue(result && result2);
			}
		}

		/// <summary>
		/// Method 5 tests
		/// </summary>

		[TestMethod]
		public void InsertTextInField_FieldExists_ReturnsTrue()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				var result = doc.InsertTextInField("Nome", "Pippo");

				//Assert
				Assert.IsTrue(result);
			}
		}

		[TestMethod]
		public void InsertTextInField_FieldDoesntExist_ReturnsFalse()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act
				var result = doc.InsertTextInField("Nomi", "Pippo");

				//Assert
				Assert.IsFalse(result);
			}
		}

		[TestMethod]
		public void InsertTextInField_InsertingTextTwice_ReturnsTrue()
		{
			//Arrange
			using (PdfUtility doc = new PdfUtility(FILE_WITH_CHECKBOX, null))
			{
				//Act

				//Inserting first time
				var result = doc.InsertTextInField("Nome", "Pippo");
				//Inserting second time
				result = doc.InsertTextInField("Nome", "Pippo2");

				//Assert
				Assert.IsTrue(result);
			}
		}

		/// <summary>
		///  Method 6 tests
		/// </summary>

		[TestMethod]
		public void Save_DocumentUntouched_ReturnsTrue()
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

				Assert.IsTrue(File.Exists(filename_out));
			}
		}

		[TestMethod]
		public void Save_DocumentTouched_ReturnsTrue()
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
				doc.FlagCheckbox();
				doc.InsertTextInField("Nome", "Pippo");

				//Act
				doc.Save();

				Assert.IsTrue(File.Exists(filename_out));
			}
		}
		*/
	}
}
