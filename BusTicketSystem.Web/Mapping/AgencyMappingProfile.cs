using AutoMapper;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Mapping
{
    public class AgencyMappingProfile : Profile
    {
        public AgencyMappingProfile()
        {
            CreateMap<Agency, AgencyResponseDTO>();

            CreateMap<AgencyOffice, OfficeResponseDTO>()
                .ForMember(dest => dest.AgencyId,
                    opt => opt.MapFrom(src => src.AgencyId ?? 0))
                .ForMember(dest => dest.OfficeAddressId,
                    opt => opt.MapFrom(src => src.OfficeAddressId ?? 0))
                .ForMember(dest => dest.FullAddress,
                    opt => opt.MapFrom(src => src.OfficeAddress == null
                        ? string.Empty
                        : $"{src.OfficeAddress.Address1}, {src.OfficeAddress.City}, {src.OfficeAddress.State} - {src.OfficeAddress.ZipCode}"));

            CreateMap<Bus, BusResponseDTO>();

            CreateMap<Driver, DriverResponseDTO>()
                .ForMember(dest => dest.OfficeId,
                    opt => opt.MapFrom(src => src.OfficeId ?? 0))
                .ForMember(dest => dest.AddressId,
                    opt => opt.MapFrom(src => src.AddressId ?? 0));
        }
    }
}
