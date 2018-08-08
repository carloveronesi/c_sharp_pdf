﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace IDSign.PdfUtility
{
	public class DocumentHasNoFieldsException : Exception
	{
		public DocumentHasNoFieldsException()
		{}
		public DocumentHasNoFieldsException(string documentName) : base("Document named \"" + documentName + "\" has no fields.")
		{}
		public DocumentHasNoFieldsException(string documentName, Exception innerException)
		: base("Document named \"" + documentName + "\" has no fields.", innerException)
		{}
		protected DocumentHasNoFieldsException(SerializationInfo info, StreamingContext ctxt)
			: base(info, ctxt)
		{ }
	}
}
