using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace IDSign.PdfUtility
{
	public class FieldNotFoundException : Exception
	{
		public FieldNotFoundException()
		{}
		public FieldNotFoundException(string name) : base("Field named \"" + name + "\" not found: ")
		{}
		public FieldNotFoundException(string name, int type) : base("Field of type \"" + PdfUtility.GetFormType(type) + "\" named \"" + name + "\" not found.")
		{ }
		public FieldNotFoundException(string message, Exception innerException)
		: base("Field not found: " + message, innerException)
		{}
		protected FieldNotFoundException(SerializationInfo info, StreamingContext ctxt)
			: base(info, ctxt)
		{ }
	}
}
