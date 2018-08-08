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
		public FieldNotFoundException(string message) : base("Field not found: " + message)
		{}
		public FieldNotFoundException(string message, Exception innerException)
		: base("Field not found: " + message, innerException)
		{}
		protected FieldNotFoundException(SerializationInfo info, StreamingContext ctxt)
			: base(info, ctxt)
		{ }
	}
}
