﻿using System;
using System.Runtime.Serialization;

namespace IDSign.PdfUtility
{
	public class RadiobuttonValueNotFoundException : Exception
	{
		public RadiobuttonValueNotFoundException()
		{}
		public RadiobuttonValueNotFoundException(string fieldName, string value) : base("Radiobutton named \"" + fieldName + "\" has no value \"" + value + "\".")
		{}
		public RadiobuttonValueNotFoundException(string fieldName, string value, string documentName, Exception innerException)
		: base("Radiobutton named \"" + fieldName + "\" has no value \"" + value + "\".", innerException)
		{}
		protected RadiobuttonValueNotFoundException(SerializationInfo info, StreamingContext ctxt)
			: base(info, ctxt)
		{}
	}
}
