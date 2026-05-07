using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services;

public interface IBookingService
{
    Task<BookingResponseDTO> CreateBookingAsync(BookingRequestDTO request);
    Task<BookingResponseDTO> GetBookingByIdAsync(int bookingId);
    Task<List<BookingResponseDTO>> GetMyBookingsAsync(int customerId);
    Task<List<BookingResponseDTO>> GetBookingsByTripAsync(int tripId);
    Task<List<int>> GetAvailableSeatsAsync(int tripId);
}