﻿using System.Diagnostics;
using System.Globalization;
using AutoMapper;
using CsvHelper;
using Intuit.Ipp.Data;
using Intuit.Ipp.OAuth2PlatformClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace qbo_dotNET.Logic
{
    public class CsvHandler : ICsvHandler
    {
        public string? rawData { get; set; }
        public List<Invoice> finalInvoiceList { get; set; }
        private readonly IApiHandler _api;
        private ILogger<CsvHandler> _logger;

        public CsvHandler(IApiHandler api, ILogger<CsvHandler> logger) { _api = api; finalInvoiceList = new(); _logger = logger; }

        public async System.Threading.Tasks.Task formatData()
        {
            try
            {
                _logger.LogWarning("Beginning to format data");
                _logger.LogWarning(rawData);

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

                        customer = await validateCustomer(lastRow);

                        if (customer.DisplayName.Equals("Bold Bean Jax Beach") || customer.DisplayName.Equals("Bold Bean Riverside"))
                        {
                            DiscountLineDetail discountLineDetail = new() { DiscountPercent = 15, DiscountPercentSpecified = true, PercentBased = true, PercentBasedSpecified = true };
                            Line discountLine = new();
                            discountLine.DetailType = LineDetailTypeEnum.DiscountLineDetail;
                            discountLine.DetailTypeSpecified = true;
                            discountLine.AnyIntuitObject = discountLineDetail;
                            lines.Add(discountLine);
                        }

                        customer.BillAddr = invoice.BillAddr;
                        customer.ShipAddr = invoice.ShipAddr;
                        invoice.CustomerRef = new ReferenceType { Value = customer.Id, name = customer.DisplayName };
                        invoice.Line = lines.ToArray<Line>();
                        invoice.AllowOnlineACHPaymentSpecified = true;
                        invoice.AllowOnlineACHPayment = true;

                        var jsonString = JsonConvert.SerializeObject(
              invoice, Formatting.Indented,
           new JsonConverter[] { new StringEnumConverter() });

                        _logger.LogWarning("Added invoice " + jsonString);
                        finalInvoiceList.Add(invoice);
                    }

                    finalInvoiceList.Reverse();
                    _logger.LogWarning("Done sorting items and lines");
                    watch.Stop();
                    TimeSpan ts = watch.Elapsed;
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);

                    _logger.LogWarning("sorted in: " + elapsedTime);

                }
            }
            catch (Exception ex) { _logger.LogWarning(ex.Data + ex.Message + ex.StackTrace); }
        }

        public async Task<Item> validateItem(CsvRow row)
        {
            Item item = new();
            try
            {
                _logger.LogError(row.LineDesc);
                if (_api.itemDictionary.TryGetValue(row.LineDesc, out item))
                {
                    bool updated = false;

                    //_logger.LogWarning("Found, " + item.Name);
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
                        // await _api.updateItemDictionary();
                        _api.itemDictionary[item.Name] = item;
                        
                    }
                }
                else
                {
                    _logger.LogWarning("Not found, " + row.LineDesc);
                    item = new();
                    item.sparse = true;
                    item.TypeSpecified = true;
                    item.Name = row.LineDesc;
                    item.Type = ItemTypeEnum.NonInventory;
                    item.IncomeAccountRef = new ReferenceType() { Value = "1" };
                    item.UnitPrice = (decimal.Parse(row.LineUnitPrice));

                    try
                    {
                        item = await _api.updateItem(item);
                    } catch (Exception ex) {_logger.LogError($"Error updating item: {ex.Message} {ex.StackTrace} {ex.Data}");}

                    // await _api.updateItemDictionary();
                    _api.itemDictionary.Add(item.Name, item);
                }
                return item;
            }
            catch (Exception ex) { _logger.LogWarning(ex.Data + ex.Message + ex.StackTrace + item.Name); }
            return new Item();
        }


        public async System.Threading.Tasks.Task<Customer> validateCustomer(CsvRow row)
        {
            Customer customer = new();
            _logger.LogError(row.Customer);
            if (_api.customerDictionary.TryGetValue(row.Customer, out customer))
            {
                bool updated = false;
                _logger.LogWarning("Found customer, " + customer.DisplayName);

                if (customer.PrimaryEmailAddr?.Address != row.BillEmailCsv)
                {
                    customer.PrimaryEmailAddr.Address = row.BillEmailCsv;
                }
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
                    // await _api.updateCustomerDictionary();
                    _api.customerDictionary[customer.DisplayName] = customer;
                }
            }
            else
            {
                customer = new();
                _logger.LogWarning("Not found, " + row.Customer);
                
                customer.DisplayName = row.Customer;
                customer.BillAddr = row.BillAddr;
                customer.ShipAddr = row.ShipAddr;
                try
                {
                    if (row.BillEmailCsv != null) customer.PrimaryEmailAddr = new() { Address = row.BillEmailCsv };
                } catch (Exception ex) {_logger.LogError($"bruh another one \n{ex.Message}\n{ex.StackTrace}\n{ex.Data}");}

                Task<Customer> returnedCustomerResult = _api.updateCustomer(customer);
                customer = await returnedCustomerResult;
                // await _api.updateCustomerDictionary();
                _api.customerDictionary.Add(customer.DisplayName, customer);

            }
            return customer;
        }
    }

}

