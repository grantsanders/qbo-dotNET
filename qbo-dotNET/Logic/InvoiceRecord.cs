using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace qbo_dotNET.Logic
{
    public class InvoiceRecord
    {
        public InvoiceRecord() { }

        private string id = "";
        private bool importedSuccessfully { get; set; }
        private string numberOfInvoices { get; set; }
        private string created = "";
        private string note = "";

    }
}

