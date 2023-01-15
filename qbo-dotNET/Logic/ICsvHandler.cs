using System;
using Intuit.Ipp.Data;

namespace qbo_dotNET.Logic
{
	public interface ICsvHandler
	{
		public List<Invoice> finalInvoiceList { get; set; }
		public string? rawData { get; set; }
        public async System.Threading.Tasks.Task formatData() { }

        public async System.Threading.Tasks.Task<Item> validateItem(CsvRow row) { return null; }
        public async System.Threading.Tasks.Task<Item> validateCustomer(CsvRow row) { return null; }

    }
}

