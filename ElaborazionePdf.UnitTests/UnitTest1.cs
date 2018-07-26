using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ElaborazionePdf.UnitTests
{
	[TestClass]
	public class UnitTest1
	{
		[TestMethod]
		public void TestMethod1()
		{
			var doc = new PdfDocument(@"a.pdf");

			var type = doc.GetAcrofieldType("Nome");

			Assert.IsTrue(type > -1);
		}
	}
}
