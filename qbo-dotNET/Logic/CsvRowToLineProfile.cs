using System;
using AutoMapper;
using Intuit.Ipp.Data;

namespace qbo_dotNET.Logic
{
    public class CsvRowToLineProfile : Profile
    {
        public CsvRowToLineProfile()
        {
            CreateMap<CsvRow, Line>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.LineDesc))
                .ForMember(dest => dest.DetailType, opt => opt.MapFrom(src => src.salesItemLineDetail))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => (Double.Parse(src.LineUnitPrice) * Double.Parse(src.LineQty)).ToString()));
        }
    }
}

