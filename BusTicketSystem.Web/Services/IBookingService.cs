using BusTicketSystem.Web.ApiResponse;
using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services;

public interface IBookingService
{
    Task<ApiResponse<BookingResponseDTO>> CreateBookingAsync(BookingRequestDTO request);
    Task<ApiResponse<BookingResponseDTO>> GetBookingByIdAsync(int bookingId);
    Task<ApiResponse<List<BookingResponseDTO>>> GetMyBookingsAsync(int customerId);
    Task<ApiResponse<List<BookingResponseDTO>>> GetBookingsByTripAsync(int tripId);
    Task<ApiResponse<List<int>>> GetAvailableSeatsAsync(int tripId);
}