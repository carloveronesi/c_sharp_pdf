using System;
using System.Runtime.Serialization;

namespace IDSign.PdfUtility
{
	public class DocumentHasNoFieldsException : Exception
	{
		public DocumentHasNoFieldsException() : base("An opened Document has no fields.")
		{ }
		public DocumentHasNoFieldsException(string documentName) : base("Document named \"" + documentName + "\" has no fields.")
		{}
		public DocumentHasNoFieldsException(string documentName, Exception innerException)
		: base("Document named \"" + documentName + "\" has no fields.", innerException)
		{}
		protected DocumentHasNoFieldsException(SerializationInfo info, StreamingContext ctxt)
			: base(info, ctxt)
		{}
	}
}
