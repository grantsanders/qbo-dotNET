using System;
using System.Globalization;
using CsvHelper;
using Intuit.Ipp.Data;



namespace qbo_dotNET.Logic
{
    public class CsvHandler : ICsvHandler
    {
        public string? rawData { get; set; }
        private readonly IApiHandler _api;

        public CsvHandler(IApiHandler api)
        {
            _api = api;
        }

        public  void formatData()
        {

            using (StringReader reader = new StringReader(rawData))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<CsvRowMap>();
                var records = csv.GetRecords<CsvRow>().ToList();

                var groups = records.GroupBy(row => row.RefNumber);

                List<Invoice> finalInvoiceList = new();

                foreach (KeyValuePair<string, Item> kvp in _api.itemDictionary)
                {
                    Console.WriteLine("Key = {0}, Value = {1}", kvp.Key, kvp.Value);
                }

                //foreach (var group in groups)
                //{
                //    int refNumber = group.Key;
                //    List<CsvRow> rowsByRef = group.ToList();
                //    Invoice invoice = new();
                //    List<Line> lines = new();

                //    foreach (var row in rowsByRef)
                //    {

                //    }
                //}

            }

        }
    }

}

