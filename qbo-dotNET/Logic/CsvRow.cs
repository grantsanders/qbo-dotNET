using System;
using Intuit.Ipp.Data;

namespace qbo_dotNET.Logic
{
    public class CsvRow
    {
        public CsvRow()
        {
            referenceType.name = LineDesc;
            ShipAddr = new()
            {
                Line1 = ShipAddrLine1,
                Line2 = ShipAddrLine2,
                Line3 = ShipAddrLine3,
                City = ShipAddrCity,
                PostalCode = ShipAddrPostalCode,
                Country = ShipAddrCountry
            };
            BillAddr = new()
            {
                Line1 = BillAddrLine1,
                Line2 = BillAddrLine2,
                Line3 = BillAddrLine3,
                City = BillAddrCity,
                PostalCode = BillAddrPostalCode,
                Country = BillAddrCountry
            };
            BillEmail = new()
            { Address = BillEmailCsv };

            CustomerMemo = new()
            {
                Value = Msg
            };

            taxLineDetail = new()
            {
                TaxPercent = 0
            };

            CustomerRef = new()
            {
                name = Customer
            };
        }

        public int RefNumber { get; set; }
        public string? Customer { get; set; }
        public string? TxnDate { get; set; }
        public string? ShipDate { get; set; }
        public string? BillAddrLine1 { get; set; }
        public string? BillAddrLine2 { get; set; }
        public string? BillAddrLine3 { get; set; }
        public string? BillAddrCity { get; set; }
        public string? BillAddrState { get; set; }
        public string? BillAddrPostalCode { get; set; }
        public string? BillAddrCountry { get; set; }
        public string? ShipAddrLine1 { get; set; }
        public string? ShipAddrLine2 { get; set; }
        public string? ShipAddrLine3 { get; set; }
        public string? ShipAddrCity { get; set; }
        public string? ShipAddrState { get; set; }
        public string? ShipAddrPostalCode { get; set; }
        public string? ShipAddrCountry { get; set; }
        public string? PrivateNote { get; set; }
        public string? Msg { get; set; }
        public string? BillEmailCsv { get; set; }
        public string? LineItem { get; set; }
        public string? LineUom { get; set; }
        public string? LineQty { get; set; }
        public string? LineDesc { get; set; }
        public string? LineUnitPrice { get; set; }
        public string? LineTaxable { get; set; }
        public string? LineClass { get; set; }
        public string? Type { get; set; }
        public string? PrintSalesOrder { get; set; }
        public string? PONumber { get; set; }
        public string? CustomerAccountNotes { get; set; }

        //intuit specific objects
        public LineDetailTypeEnum salesItemLineDetail = LineDetailTypeEnum.SalesItemLineDetail;
        public TaxLineDetail taxLineDetail = new();
        public ReferenceType CustomerRef = new();
        public ReferenceType referenceType = new();
        public PhysicalAddress BillAddr { get; set; }
        public PhysicalAddress ShipAddr { get; set; }
        public MemoRef CustomerMemo { get; set; }
        public EmailAddress BillEmail { get; set; }
        public bool DetailTypeSpecified = true;
        public bool AmountSpecified = true;
        public bool AllowOnlineACHPayment = true;
    }

}

