using System;
using AutoMapper;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;

namespace BusTicketSystem.Web.Services;

public class TripService : ITripService
{
    private readonly ITripRepository _tripRepo;
    private readonly IRouteRepository _routeRepo;
    private readonly IMapper _mapper;
    public TripService(ITripRepository tripRepo, IRouteRepository routeRepo, IMapper mapper)
    {
        _mapper = mapper;
        _tripRepo = tripRepo;
        _routeRepo = routeRepo;
    }
    public async Task<TripSummaryDTO> CreateTripAsync(CreateTripRequestDTO request)
    {
        if (!await _routeRepo.ExistsAsync(request.RouteId))
        {
            throw new NotFoundException($"Route with Id {request.RouteId} does not exist.");
        }
        var trip = _mapper.Map<Trip>(request);
        
        // Fetch bus to get capacity
        var bus = await _tripRepo.GetBusByIdAsync(request.BusId);
        if (bus != null)
        {
            trip.AvailableSeats = bus.Capacity;
        }

        var createTrip = await _tripRepo.AddAsync(trip);
        return _mapper.Map<TripSummaryDTO>(createTrip);
    }

    public async Task<IEnumerable<TripSummaryDTO>> GetAgencyTripsAsync(int agencyId)
    {
        var trips = await _tripRepo.GetTripsByAgencyAsync(agencyId);
        return _mapper.Map<IEnumerable<TripSummaryDTO>>(trips);
    }

    public async Task<IEnumerable<MyTripWithOccupancyDTO>> GetMyTripsWithOccupancyAsync(int agencyId)
    {
        var trips = await _tripRepo.GetTripsByAgencyAsync(agencyId);
        var result = new List<MyTripWithOccupancyDTO>();

        foreach (var trip in trips)
        {
            var bookings = await _tripRepo.GetBookingsByTripIdAsync(trip.TripId);
            var bookedCount = bookings.Count(b => b.Status != "Available");

            var dto = _mapper.Map<MyTripWithOccupancyDTO>(trip);
            dto.TotalSeats = trip.Bus.Capacity;
            dto.BookedSeats = bookedCount;
            dto.AvailableSeats = trip.AvailableSeats;
            dto.OccupancyPercentage = (double)bookedCount / trip.Bus.Capacity * 100;

            result.Add(dto);
        }
        return result;
    }

    public async Task<SeatLayoutDTO?> GetSeatLayoutAsync(int tripId)
    {
        var trip = await _tripRepo.GetByIdAsync(tripId);
        if (trip == null) throw new NotFoundException($"Trip with ID {tripId} not found.");

        var bookings = await _tripRepo.GetBookingsByTripIdAsync(tripId);

        var seatLayout = new SeatLayoutDTO
        {
            TripId = tripId,
            TotalSeats = trip.Bus.Capacity,
            AvailableSeats = trip.AvailableSeats,
            Seats = new List<SeatDTO>()
        };

        var bookedSeats = bookings
            .Where(b => b.Status != "Available")
            .Select(b => b.SeatNumber)
            .ToHashSet();

        for (int i = 1; i <= trip.Bus.Capacity; i++)
        {
            seatLayout.Seats.Add(new SeatDTO
            {
                SeatNumber = i,
                Status = bookedSeats.Contains(i) ? "Booked" : "Available"
            });
        }

        return seatLayout;
    }

    public async Task<TripDetailDTO?> GetTripDetailsAsync(int id)
    {
        var trip = await _tripRepo.GetByIdAsync(id);
        if (trip == null) throw new NotFoundException("trip not found");

        var dto = _mapper.Map<TripDetailDTO>(trip);

        var boardingAddress = await _tripRepo.GetAddressByIdAsync(trip.BoardingAddressId);
        if (boardingAddress != null) dto.BoardingAddress = _mapper.Map<AddressMiniDTO>(boardingAddress);

        return dto;
    }

    public async Task<IEnumerable<TripSearchResultDTO>> SearchTripsAsync(string? fromCity, string? toCity, DateTime? date)
    {
        var trips = await _tripRepo.SearchTripsAsync(fromCity, toCity, date);
        return _mapper.Map<IEnumerable<TripSearchResultDTO>>(trips);
    }

    public async Task<TripSummaryDTO> UpdateTripAsync(int id, UpdateTripRequestDTO request)
    {
        var trip = await _tripRepo.GetByIdAsync(id);
        if (trip == null) throw new NotFoundException("Trip Not found");

        _mapper.Map(request, trip);
        var updatedTrip = await _tripRepo.UpdateAsync(trip);
        return _mapper.Map<TripSummaryDTO>(updatedTrip);
    }
}
