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
        public string? rawData { get; set; }
        public List<Invoice> finalInvoiceList { get; set; }
        private readonly IApiHandler _api;

        public CsvHandler(IApiHandler api) { _api = api; finalInvoiceList = new(); }

        public void test(object obj)
        {
            var jsonString = JsonConvert.SerializeObject(obj, Formatting.Indented, new JsonConverter[] { new StringEnumConverter() });
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

                foreach (var group in groups)
                {
                    var invoiceMapperConfig = new MapperConfiguration(cfg => { cfg.AddProfile<CsvRowToInvoiceProfile>(); });
                    var invoiceMapper = invoiceMapperConfig.CreateMapper();
                    var lineMapperConfig = new MapperConfiguration(cfg => { cfg.AddProfile<CsvRowToLineProfile>(); });
                    var lineMapper = lineMapperConfig.CreateMapper();

                    int refNumber = group.Key;
                    List<CsvRow> rowsByRef = group.ToList();

                    List<Line> lines = new();
                    Invoice invoice = new();
                    Customer customer = new();
                    CsvRow lastRow = new();

                    foreach (var row in rowsByRef)
                    {
                        invoice = invoiceMapper.Map<Invoice>(row);
                        Line line = lineMapper.Map<Line>(row);

                        Item item = validateItem(row).Result;

                        SalesItemLineDetail salesItemLineDetail = new() { Qty = (decimal.Parse(row.LineQty)), QtySpecified = true };
                        salesItemLineDetail.ItemRef = new ReferenceType { Value = item.Id };
                        line.DetailType = LineDetailTypeEnum.SalesItemLineDetail;
                        line.DetailTypeSpecified = true;
                        line.AnyIntuitObject = salesItemLineDetail;
                        lines.Add(line);
                        lastRow = row;
                    }

                    Task<Customer> returnedCustomerResult = validateCustomer(lastRow);
                    customer = await returnedCustomerResult;

                    if (customer.DisplayName.Equals("Bold Bean Jax Beach") || customer.DisplayName.Equals("Bold Bean Riverside"))
                    {
                        DiscountLineDetail discountLineDetail = new() { DiscountPercent = 15, DiscountPercentSpecified = true, PercentBased = true, PercentBasedSpecified = true };
                        Line discountLine = new();
                        discountLine.DetailType = LineDetailTypeEnum.DiscountLineDetail;
                        discountLine.DetailTypeSpecified = true;
                        discountLine.AnyIntuitObject = discountLineDetail;
                        lines.Add(discountLine);
                    }

                    invoice.CustomerRef = new ReferenceType { Value = customer.Id, name = customer.DisplayName };
                    invoice.Line = lines.ToArray<Line>();

                    Console.WriteLine(invoice.Line[0].Amount);
                    finalInvoiceList.Add(invoice);
                }

                Console.WriteLine("Done sorting items and lines");
                watch.Stop();

                TimeSpan ts = watch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds / 10);
                Console.WriteLine("sorted in: " + elapsedTime);
            }
        }



        public async Task<Item> validateItem(CsvRow row)
        {
            Item item = new();

            if (_api.itemDictionary.TryGetValue(row.LineDesc, out item))
            {
                bool updated = false;

                Console.WriteLine("Found, " + item.Name);
                if (!item.Active)
                {
                    item.Active = true;
                    item.sparse = true;
                    updated = true;
                }
                if (item.UnitPrice != (decimal.Parse(row.LineUnitPrice)))
                {
                    item.UnitPrice = (decimal.Parse(row.LineUnitPrice));
                    updated = true;
                }

                if (updated == true)
                {
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
                item.IncomeAccountRef = new ReferenceType() { Value = "85" };
                item.UnitPrice = (decimal.Parse(row.LineUnitPrice));

                item = _api.updateItem(item).Result;
                //Task<Item> returnedItemResult = _api.updateItem(item);
                //item = await returnedItemResult;
                _api.getWorkingLists();
            }
            return item;
        }

        public async System.Threading.Tasks.Task<Customer> validateCustomer(CsvRow row)
        {
            Customer customer = new();

            if (_api.customerDictionary.TryGetValue(row.Customer, out customer))
            {
                bool updated = false;

                Console.WriteLine("Found customer, " + customer.DisplayName);
                if (customer.BillAddr != row.BillAddr || customer.ShipAddr != row.ShipAddr)
                {
                    customer.BillAddr = row.BillAddr;
                    customer.ShipAddr = row.ShipAddr;
                    updated = true;
                }
                if (!customer.Active)
                {
                    customer.Active = true;
                    updated = true;
                }

                if (updated == true)
                {
                    Task<Customer> returnedCustomerResult = _api.updateCustomer(customer);
                    customer = await returnedCustomerResult;
                    _api.updateCustomerDictionary();

                }
            }
            else
            {
                customer = new();
                Console.WriteLine("Not found, " + row.Customer);
                customer.DisplayName = row.Customer;
                customer.BillAddr = row.BillAddr;
                customer.ShipAddr = row.ShipAddr;
                Task<Customer> returnedCustomerResult = _api.updateCustomer(customer);
                customer = await returnedCustomerResult;
                _api.updateCustomerDictionary();

            }
            //_api.updateCustomerDictionary();
            return customer;
        }


    }

}

