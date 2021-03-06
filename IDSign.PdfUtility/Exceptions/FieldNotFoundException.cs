﻿using System;
using System.Runtime.Serialization;

namespace IDSign.PdfUtility
{
	public class FieldNotFoundException : Exception
	{
		public FieldNotFoundException()
		{}
		public FieldNotFoundException(string name) : base("Field named \"" + name + "\" not found.")
		{}
		public FieldNotFoundException(string name, int type) : base("Field of type \"" + PdfUtility.GetFormType(type) + "\" named \"" + name + "\" not found.")
		{ }
		public FieldNotFoundException(string name, Exception innerException)
		: base("Field named \"" + name + "\" not found.", innerException)
		{}
		protected FieldNotFoundException(SerializationInfo info, StreamingContext ctxt)
			: base(info, ctxt)
		{ }
	}
}
