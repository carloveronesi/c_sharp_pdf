using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace IDSign.PdfUtility
{
	public class RadiobuttonValueNotFoundException : Exception
	{
		public RadiobuttonValueNotFoundException()
		{}
		public RadiobuttonValueNotFoundException(string fieldName, string value, string documentName) : base("Radiobutton named \"" + fieldName + "\" in document \"" + documentName + "\" has no value \"" + value + "\".")
		{}
		public RadiobuttonValueNotFoundException(string fieldName, string value, string documentName, Exception innerException)
		: base("Radiobutton named \"" + fieldName + "\" in document \"" + documentName + "\" has no value \"" + value + "\".", innerException)
		{}
		protected RadiobuttonValueNotFoundException(SerializationInfo info, StreamingContext ctxt)
			: base(info, ctxt)
		{}
	}
}
