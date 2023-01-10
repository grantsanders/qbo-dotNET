using System;
using AutoMapper;
using Intuit.Ipp.Data;

namespace qbo_dotNET.Logic
{
    public class CsvRowToInvoiceProfile : Profile
    {
        public CsvRowToInvoiceProfile()
        {
            CreateMap<CsvRow, Invoice>()
                .ForMember(dest => dest.TxnDate, opt => opt.MapFrom(src => src.TxnDate))
                .ForMember(dest => dest.CustomerMemo, opt => opt.MapFrom(src => src.Msg))
                .ForMember(dest => dest.BillEmail, opt => opt.MapFrom(src => src.BillEmail))
                .ForMember(dest => dest.ShipAddr, opt => opt.MapFrom(src => src.ShipAddr))
                .ForMember(dest => dest.AllowOnlineACHPayment, opt => opt.MapFrom(src => src.AllowOnlineACHPayment))
                .ForMember(dest => dest.BillAddr, opt => opt.MapFrom(src => src.BillAddr));
        }
    }
}

