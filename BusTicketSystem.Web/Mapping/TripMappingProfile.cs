using System;
using System.Runtime.InteropServices;
using AutoMapper;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Mapping;

public class TripMappingProfile : Profile
{
    public TripMappingProfile()
    {
        CreateMap<Models.Route, RouteResponseDTO>()
            .ForMember(dest => dest.EstimatedDurationMinutes, opt => opt.MapFrom(src => src.Duration))
            .ForMember(dest => dest.BreakPoints, opt => opt.MapFrom(src => src.BreakPoints.HasValue ? src.BreakPoints.Value.ToString() : null));

        CreateMap<Models.Route, RouteSearchResultDTO>()
            .ForMember(dest => dest.EstimatedDurationMinutes, opt => opt.MapFrom(src => src.Duration));

        CreateMap<CreateRouteRequestDTO, Models.Route>()
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.EstimatedDurationMinutes))
            .ForMember(dest => dest.BreakPoints, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.BreakPoints) ? (int?)int.Parse(src.BreakPoints) : null));

        CreateMap<UpdateRouteRequestDTO, Models.Route>()
            .ForMember(dest => dest.Duration, opt => opt.MapFrom(src => src.EstimatedDurationMinutes))
            .ForMember(dest => dest.BreakPoints, opt => opt.MapFrom(src => !string.IsNullOrEmpty(src.BreakPoints) ? (int?)int.Parse(src.BreakPoints) : null))
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Trip, TripSummaryDTO>()
            .ForMember(dest => dest.FromCity, opt => opt.MapFrom(src => src.Route.FromCity))
            .ForMember(dest => dest.ToCity, opt => opt.MapFrom(src => src.Route.ToCity))
            .ForMember(dest => dest.TotalSeats, opt => opt.MapFrom(src => src.Bus.Capacity));

        CreateMap<Trip, TripDetailDTO>()
            .ForMember(dest => dest.FromCity, opt => opt.MapFrom(src => src.Route.FromCity))
            .ForMember(dest => dest.ToCity, opt => opt.MapFrom(src => src.Route.ToCity))
            .ForMember(dest => dest.BreakPoints, opt => opt.MapFrom(src => src.Route.BreakPoints.HasValue ? src.Route.BreakPoints.Value.ToString() : null))
            .ForMember(dest => dest.EstimatedDurationMinutes, opt => opt.MapFrom(src => src.Route.Duration))
            .ForMember(dest => dest.TotalSeats, opt => opt.MapFrom(src => src.Bus.Capacity))
            .ForMember(dest => dest.Driver, opt => opt.MapFrom(src => src.Driver1Driver));

        CreateMap<Trip, TripSearchResultDTO>()
            .ForMember(dest => dest.FromCity, opt => opt.MapFrom(src => src.Route.FromCity))
            .ForMember(dest => dest.ToCity, opt => opt.MapFrom(src => src.Route.ToCity))
            .ForMember(dest => dest.BusType, opt => opt.MapFrom(src => src.Bus.Type))
            .ForMember(dest => dest.AgencyName, opt => opt.MapFrom(src => src.Bus.Office.Agency.Name));

        CreateMap<Trip, MyTripWithOccupancyDTO>()
            .ForMember(dest => dest.FromCity, opt => opt.MapFrom(src => src.Route.FromCity))
            .ForMember(dest => dest.ToCity, opt => opt.MapFrom(src => src.Route.ToCity))
            .ForMember(dest => dest.TotalSeats, opt => opt.MapFrom(src => src.Bus.Capacity));

        CreateMap<CreateTripRequestDTO, Trip>()
            .ForMember(dest => dest.Driver1DriverId, opt => opt.MapFrom(src => src.DriverId))
            .ForMember(dest => dest.TripDate, opt => opt.MapFrom(src => src.DepartureTime.Date));

        CreateMap<UpdateTripRequestDTO, Trip>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Agency, AgencyMiniDTO>();
        CreateMap<Bus, BusMiniDTO>()
            .ForMember(dest => dest.TotalCapacity, opt => opt.MapFrom(src => src.Capacity))
            .ForMember(dest => dest.BusType, opt => opt.MapFrom(src => src.Type));
        CreateMap<Driver, DriverMiniDTO>();
        CreateMap<Address, AddressMiniDTO>()
            .ForMember(dest => dest.Street, opt => opt.MapFrom(src => src.Address1))
            .ForMember(dest => dest.PinCode, opt => opt.MapFrom(src => src.ZipCode));
    }
}
