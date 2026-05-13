using BusTicketSystem.Web.ResponseWrapper;
using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services;

public interface IBookingService
{
    Task<ApiResponse<BookingResponseDTO>> CreateBookingAsync(int customerId, BookingRequestDTO request);
    Task<ApiResponse<BookingResponseDTO>> GetBookingByIdAsync(int bookingId);
    Task<ApiResponse<List<BookingResponseDTO>>> GetMyBookingsAsync(int customerId);
    Task<ApiResponse<List<BookingResponseDTO>>> GetBookingsByTripAsync(int tripId);
    Task<ApiResponse<List<int>>> GetAvailableSeatsAsync(int tripId);
    Task<ApiResponse<string>> CancelBookingAsync(int customerId, int bookingId);
}