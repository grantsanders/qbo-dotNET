using System;
using System.Diagnostics;
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
        public string rawData { get; set; }
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

        public async System.Threading.Tasks.Task formatData()
        {

            Stopwatch watch = new();
            watch.Start();


            using (StringReader reader = new StringReader(rawData))
            using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
            {
                csv.Context.RegisterClassMap<CsvRowMap>();
                var records = csv.GetRecords<CsvRow>().ToList();
                var groups = records.GroupBy(row => row.RefNumber);
                List<Invoice> finalInvoiceList = new();


                foreach (var group in groups)
                {


                    var invoiceMapperConfig = new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile<CsvRowToInvoiceProfile>();
                    });
                    var invoiceMapper = invoiceMapperConfig.CreateMapper();


                    var lineMapperConfig = new MapperConfiguration(cfg =>
                    {
                        cfg.AddProfile<CsvRowToLineProfile>();
                    });


                    var lineMapper = lineMapperConfig.CreateMapper();
                    int refNumber = group.Key;
                    List<CsvRow> rowsByRef = group.ToList();
                    List<Line> lines = new();
                    Invoice invoice = new();


                    foreach (var row in rowsByRef)
                    {

                        try
                        {
                            invoice = invoiceMapper.Map<Invoice>(row);
                        } catch (AutoMapperMappingException ex) { Console.WriteLine(ex.Message + "\n" + ex.Data); }

                        Line line = lineMapper.Map<Line>(row);
                        Item item = new();

                        if (_api.itemDictionary.TryGetValue(row.LineDesc, out item))
                        {

                            Console.WriteLine("Found, " + item.Name);
                            if (!item.Active)
                            {
                                item.Active = true;
                                item.sparse = true;
                                Task<Item> returnedItemResult = _api.updateItem(item);
                                item = await returnedItemResult;
                            }
                            if (item.UnitPrice != (decimal.Parse(row.LineUnitPrice)))
                            {
                                item.UnitPrice = (decimal.Parse(row.LineUnitPrice));
                                Task<Item> returnedItemResult = _api.updateItem(item);
                                item = await returnedItemResult;
                            }
                        }

                        else
                        {
                            Console.WriteLine("Bong");
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

                            Task<Item> returnedItemResult = _api.updateItem(item);
                            item = await returnedItemResult;
                            _api.getWorkingLists();
                        }

                        line.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        line.DetailTypeSpecified = true;

                        SalesItemLineDetail salesItemLineDetail = new()
                        {
                            Qty = (decimal.Parse(row.LineQty)),
                            QtySpecified = true
                        };

                        salesItemLineDetail.ItemRef = new ReferenceType
                        {
                            Value = item.Id,
                        };
                        //tag line with item info
                        line.AnyIntuitObject = salesItemLineDetail;
                        lines.Add(line);



                    }

                    invoice.Line = lines.ToArray();

                    test(invoice);

                    Console.WriteLine("\n\n\n");
                }
                Console.WriteLine("Done sorting items and lines");
                watch.Stop();

                TimeSpan ts = watch.Elapsed;

                // Format and display the TimeSpan value.
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Console.WriteLine("Processed all items in " + elapsedTime);
            }

        }
    }

}

