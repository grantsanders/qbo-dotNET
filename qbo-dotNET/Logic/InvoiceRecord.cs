using System;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace qbo_dotNET.Logic
{
    public class InvoiceRecord
    {
        public InvoiceRecord() { }

        public string? Id { get; set; }
        public bool? ImportedSuccessfully { get; set; }
        public string? NumberOfInvoices { get; set; }
        public string? Created { get; set; }
        public string? Note { get; set; }

    }
}

