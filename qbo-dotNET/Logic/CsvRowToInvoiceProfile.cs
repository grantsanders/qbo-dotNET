//using System;
//using AutoMapper;
//using Intuit.Ipp.Data;

//namespace qbo_dotNET.Logic
//{
//	public class CsvRowToInvoiceProfile : Profile
//	{
//        public CsvRowToInvoiceProfile()
//        {
//            CreateMap<CsvRow, Invoice>()
//                .ForMember(dest => dest.TxnDate, opt => opt.MapFrom(src => src.TxnDate))
//                .ForMember(dest => dest.TotalAmt, opt => opt.MapFrom(src => src.TotalAmt))
//                .ForMember(dest => dest.DocNumber, opt => opt.MapFrom(src => src.DocNumber))
//                .ForMember(dest => dest.CustomerMemo, opt => opt.MapFrom(src => src.CustomerMemo))
//                .ForMember(dest => dest.Deposit, opt => opt.MapFrom(src => src.Deposit))
//                .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Balance))
//                .ForMember(dest => dest.SyncToken, opt => opt.MapFrom(src => src.SyncToken))
//                .ForMember(dest => dest.BillEmail, opt => opt.MapFrom(src => src.BillEmail))
//                .ForMember(dest => dest.ShipAddr, opt => opt.MapFrom(src => src.ShipAddr))
//                .ForMember(dest => dest.EmailStatus, opt => opt.MapFrom(src => src.EmailStatus))
//                .ForMember(dest => dest.AllowOnlineCreditCardPayment, opt => opt.MapFrom(src => src.AllowOnlineCreditCardPayment))
//                .ForMember(dest => dest.AllowOnlineACHPayment, opt => opt.MapFrom(src => src.AllowOnlineACHPayment))
//                .ForMember(dest => dest.AllowOnlinePayment, opt => opt.MapFrom(src => src.AllowOnlinePayment))
//                .ForMember(dest => dest.BillAddr, opt => opt.MapFrom(src => src.BillAddr))
//                .ForMember(dest => dest.TxnStatus, opt => opt.MapFrom(src => src.TxnStatus

//        }
//	}
//}

