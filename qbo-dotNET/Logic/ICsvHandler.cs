using System;
using Intuit.Ipp.Data;

namespace qbo_dotNET.Logic
{
	public interface ICsvHandler
	{
		public List<Invoice> finalInvoiceList { get; set; }
		public string? rawData { get; set; }
        public void formatData() { }
		

        }
    }

