using System;
using System.Globalization;
using AutoMapper;
using CsvHelper;
using Intuit.Ipp.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace qbo_dotNET.Logic
{
    public class CsvHandler : ICsvHandler
    {
        public string? rawData { get; set; }
        private readonly IApiHandler _api;
        public List<Invoice> finalInvoiceList { get; set; }

        public CsvHandler(IApiHandler api)
        {
            _api = api;
        }

        public void test(object obj)
        {
            var jsonString = JsonConvert.SerializeObject(
obj, Formatting.Indented,
new JsonConverter[] { new StringEnumConverter() });

            Console.WriteLine(jsonString);
        }

        public void formatData()
        {

            using (StringReader reader = new StringReader(rawData))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<CsvRowMap>();
                var records = csv.GetRecords<CsvRow>().ToList();
                var groups = records.GroupBy(row => row.RefNumber);
                List<Invoice> finalInvoiceList = new();

                foreach (var group in groups)
                {
                    var config = new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile<CsvRowToLineProfile>();
                    });

                    var mapper = config.CreateMapper();
                    int refNumber = group.Key;
                    List<CsvRow> rowsByRef = group.ToList();
                    Invoice invoice = new();
                    List<Line> lines = new();

                    foreach (var row in rowsByRef)
                    {
                        Line line = mapper.Map<Line>(row);
                        Item item = new();
                        
                        if (_api.itemDictionary.TryGetValue(row.LineDesc, out item))
                        {

                            Console.WriteLine("Found, " + item.Name);
                            if (!item.Active)
                            {
                                item.Active = true;
                                item.sparse = true;
                                _api.updateItem(item);
                            }
                            if (item.UnitPrice != (decimal.Parse(row.LineUnitPrice)))
                            {
                                item.UnitPrice = (decimal.Parse(row.LineUnitPrice));
                            }
                        }
                        else
                        {
                            item = new();
                            item.sparse = true;
                            item.TypeSpecified = true;
                            item.Name = row.LineDesc;
                            item.Type = ItemTypeEnum.NonInventory;
                            item.IncomeAccountRef = new ReferenceType()
                            {
                                Value = "85",
                            };
                            item.UnitPrice = (decimal.Parse(row.LineUnitPrice));

                            item = _api.updateItem(item);  //todo: make this work
                            _api.getWorkingLists();
                        }

                        SalesItemLineDetail salesItemLineDetail = new();
                        salesItemLineDetail.ItemRef = new ReferenceType
                        {
                            Value = item.Id
                        };

                        line.AnyIntuitObject = salesItemLineDetail;

                    }

                    invoice.Line = lines.ToArray();
                    finalInvoiceList.Add(invoice);
                }
                Console.WriteLine("Done sorting items and lines");


            }

        }
    }

}

