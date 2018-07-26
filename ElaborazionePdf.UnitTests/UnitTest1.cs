using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElaborazionePdf.UnitTests
{
	[TestClass]
	public class PdfDocumentTests
	{
		/*!
		 Constructor tests
		 */

		[TestMethod]
		public void Constructor_FileExists_NoExceptions()
		{
			//Arrange
			var doc = new PdfDocument(@"TestFiles\Richiesta di adesione e Condizioni relative all'uso della firma elettronica avanzata_checkbox.pdf");
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
			var doc = new PdfDocument(@"TestFiles\Richiesta di adesione e Condizioni relative all'uso della firma elettronica avanzata_checkbox.pdf");
			
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
			var doc = new PdfDocument(@"TestFiles\Richiesta di adesione e Condizioni relative all'uso della firma elettronica avanzata_checkbox.pdf");

			//Act
			var type = doc.GetAcrofieldType("Nomi");

			//Assert
			Assert.AreEqual(-1, type);
		}
	}
}
