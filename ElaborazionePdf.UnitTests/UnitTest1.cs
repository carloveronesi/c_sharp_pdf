using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElaborazionePdf.UnitTests
{
	[TestClass]
	public class PdfDocumentTests
	{
		private const string FILE_WITH_CHECKBOX = @"TestFiles\Richiesta di adesione e Condizioni relative all'uso della firma elettronica avanzata_checkbox.pdf";
		private const string FILE_WITH_SIGNATUREFIELD = @"TestFiles\Richiesta di adesione e Condizioni relative all'uso della firma elettronica avanzata_signaturefield.pdf";
		/*!
		 Constructor tests
		 */

		[TestMethod]
		public void Constructor_FileExists_NoExceptions()
		{
			//Arrange
			var doc = new PdfDocument(FILE_WITH_CHECKBOX);
		}

		[TestMethod]
		[ExpectedException(typeof(FileNotFoundException),"No Exception found: file exists (but should not)")]
		public void Constructor_FileDoesntExist_ThrowsException()
		{
			//Arrange / Act
			var doc = new PdfDocument(@"TestFiles\filenonesistente.pdf");
		}

		[TestMethod]
		[ExpectedException(typeof(DirectoryNotFoundException), "No Exception found: directory exists (but should not)")]
		public void Constructor_DirectoryDoesntExist_ThrowsException()
		{
			//Arrange / Act
			var doc = new PdfDocument(@"DirectoryNonEsistente\filenonesistente.pdf");
		}

		/*!
		 Method 1 tests
		 */

		[TestMethod]
		public void GetAcrofieldType_FieldExists_ReturnsFieldType()
		{
			//Arrange
			var doc = new PdfDocument(FILE_WITH_CHECKBOX);
			
			//Act
			var type = doc.GetAcrofieldType("Nome");
			System.Diagnostics.Debug.WriteLine("Looking for acrofield named \"Nome\", type: " + type + " (" + PdfDocument.GetFormType(type) + ")");

			//Assert
			Assert.IsTrue(type > -1);
		}

		[TestMethod]
		public void GetAcrofieldType_FieldDoesntExist_ReturnsMinusOne()
		{
			//Arrange
			var doc = new PdfDocument(FILE_WITH_CHECKBOX);

			//Act
			var type = doc.GetAcrofieldType("Nomi");

			//Assert
			Assert.AreEqual(-1, type);
		}

		/*!
		 Method 2 tests
		 */

		[TestMethod]
		public void FlagCheckbox_CheckboxExists_ReturnsTrue()
		{
			//Arrange
			var doc = new PdfDocument(FILE_WITH_CHECKBOX);

			//Act
			var result = doc.FlagCheckbox();

			//Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void FlagCheckbox_TwoCheckableCheckboxes_ReturnsTrue()
		{
			//Arrange
			var doc = new PdfDocument(FILE_WITH_CHECKBOX);

			//Act
			//Flagging first checkbox
			var result1 = doc.FlagCheckbox();
			//Flagging second
			var result2 = doc.FlagCheckbox();

			//Assert
			Assert.IsTrue(result1 && result2);
		}

		[TestMethod]
		public void FlagCheckbox_TwoCheckableCheckboxesButTryingToCheckThree_ReturnsFalse()
		{
			//Arrange
			var doc = new PdfDocument(FILE_WITH_CHECKBOX);

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

		[TestMethod]
		public void FlagCheckbox_CheckboxDoesntExist_ReturnsFalse()
		{
			//Arrange
			var doc = new PdfDocument(FILE_WITH_SIGNATUREFIELD);

			//Act
			var result = doc.FlagCheckbox();

			//Assert
			Assert.IsFalse(result);
		}

		/*!
		 Method 3 tests
		 */

		[TestMethod]
		public void SubstituteSignature_SignatureExists_ReturnsTrue()
		{
			//Arrange
			var doc = new PdfDocument(FILE_WITH_SIGNATUREFIELD);

			//Act
			var result = doc.SubstituteSignature();

			//Assert
			Assert.IsTrue(result);
		}

		[TestMethod]
		public void SubstituteSignature_TwoSignatureFields_ReturnsTrue()
		{
			//Arrange
			var doc = new PdfDocument(FILE_WITH_SIGNATUREFIELD);

			//Act
			//Flagging first checkbox
			var result1 = doc.SubstituteSignature();
			//Flagging second
			var result2 = doc.SubstituteSignature();

			//Assert
			Assert.IsTrue(result1 && result2);
		}

		[TestMethod]
		public void SubstituteSignature_TwoSignatureFieldsButTryingToSubstituteThree_ReturnsFalse()
		{
			//Arrange
			var doc = new PdfDocument(FILE_WITH_SIGNATUREFIELD);

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

		[TestMethod]
		public void SubstituteSignature_SignatureDoesntExist_ReturnsFalse()
		{
			//Arrange
			var doc = new PdfDocument(FILE_WITH_CHECKBOX);

			//Act
			var result = doc.SubstituteSignature();

			//Assert
			Assert.IsFalse(result);
		}

	}
}
