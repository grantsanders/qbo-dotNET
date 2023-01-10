using CsvHelper.Configuration;

public sealed class CsvRowMap : ClassMap<CsvRow>
{

    public CsvRowMap()
    {
        Map(m => m.RefNumber).Name("RefNumber");
        Map(m => m.Customer).Name("Customer");
        Map(m => m.TxnDate).Name("TxnDate");
        Map(m => m.ShipDate).Name("ShipDate");
        Map(m => m.BillAddrLine1).Name("BillAddrLine1");
        Map(m => m.BillAddrLine2).Name("BillAddrLine2");
        Map(m => m.BillAddrLine3).Name("BillAddrLine3");
        Map(m => m.BillAddrCity).Name("BillAddrCity");
        Map(m => m.BillAddrState).Name("BillAddrState");
        Map(m => m.BillAddrPostalCode).Name("BillAddrPostalCode");
        Map(m => m.BillAddrCountry).Name("BillAddrCountry");
        Map(m => m.ShipAddrLine1).Name("ShipAddrLine1");
        Map(m => m.ShipAddrLine2).Name("ShipAddrLine2");
        Map(m => m.ShipAddrLine3).Name("ShipAddrLine3");
        Map(m => m.ShipAddrCity).Name("ShipAddrCity");
        Map(m => m.ShipAddrState).Name("ShipAddrState");
        Map(m => m.ShipAddrPostalCode).Name("ShipAddrPostalCode");
        Map(m => m.ShipAddrCountry).Name("ShipAddrCountry");
        Map(m => m.PrivateNote).Name("PrivateNote");
        Map(m => m.Msg).Name("Msg");
        Map(m => m.BillEmailCsv).Name("BillEmail");
        Map(m => m.LineItem).Name("LineItem");
        Map(m => m.LineUom).Name("LineUom");
        Map(m => m.LineQty).Name("LineQty");
        Map(m => m.LineDesc).Name("LineDesc");
        Map(m => m.LineUnitPrice).Name("LineUnitPrice");
        Map(m => m.LineTaxable).Name("LineTaxable");
        Map(m => m.LineClass).Name("LineClass");
        Map(m => m.Type).Name("Type");
        Map(m => m.PrintSalesOrder).Name("PrintSalesOrder");
        Map(m => m.PONumber).Name("PONumber");
        Map(m => m.CustomerAccountNotes).Name("CustomerAccountNotes");
    }
}
