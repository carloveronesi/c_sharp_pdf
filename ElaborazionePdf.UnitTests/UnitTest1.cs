using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElaborazionePdf.UnitTests
{
	[TestClass]
	public class PdfDocumentTests
	{
		/*
		[TestMethod]
		public void Constructor_FileExists_NoExceptions()
		{
			//Arrange
			var doc = new PdfDocument(@"TestFiles\Richiesta di adesione e Condizioni relative all'uso della firma elettronica avanzata_checkbox.pdf");
		}
		*/
		[TestMethod]
		[ExpectedException(typeof(IOException),
		 "File not found")]
		public void Constructor_FileDoesntExist_Exception()
		{
			//Arrange
			var doc = new PdfDocument(@"TestFiles\filenonesistente.pdf");
			Assert.Fail();
		}

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
