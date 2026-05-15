using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services;

public interface ITripService
{
    Task<IEnumerable<TripSummaryDTO>> GetAgencyTripsAsync(int agencyId);
    Task<TripDetailDTO?> GetTripDetailsAsync(int id);
    Task<IEnumerable<TripSearchResultDTO>> SearchTripsAsync(string? fromCity, string? toCity, DateTime? date);
    Task<SeatLayoutDTO?> GetSeatLayoutAsync(int tripId);
    Task<TripSummaryDTO> CreateTripAsync(CreateTripRequestDTO request);
    Task<TripSummaryDTO> UpdateTripAsync(int id, UpdateTripRequestDTO request);
    Task<IEnumerable<MyTripWithOccupancyDTO>> GetMyTripsWithOccupancyAsync(int agencyId);
}
