using BusTicketSystem.Web.ResponseWrapper;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Helper;
using BusTicketSystem.Web.Mapping;
using BusTicketSystem.Web.Repositories;

namespace BusTicketSystem.Web.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepository;

    public BookingService(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<ApiResponse<BookingResponseDTO>> CreateBookingAsync(BookingRequestDTO request)
    {
        var seat = await _bookingRepository
            .GetAvailableSeatAsync(request.TripId, request.SeatNumber);

        if (seat == null)
            return ApiResponse<BookingResponseDTO>.FailureResponse(
                "Seat is not available", statusCode: 409);

        seat.Status = "Pending";
        await _bookingRepository.UpdateAsync(seat);
        BookingTimeoutHelper.TrackBooking(seat.BookingId);

        return ApiResponse<BookingResponseDTO>.SuccessResponse(
            BookingMapper.ToDto(seat),
            "Booking successful. Please complete payment within 10 minutes.");
    }

    public async Task<ApiResponse<BookingResponseDTO>> GetBookingByIdAsync(int bookingId)
    {
        var booking = await _bookingRepository.GetByIdAsync(bookingId);

        if (booking == null)
            return ApiResponse<BookingResponseDTO>.FailureResponse(
                "Booking not found", statusCode: 404);

        return ApiResponse<BookingResponseDTO>.SuccessResponse(
            BookingMapper.ToDto(booking),
            "Booking fetched successfully");
    }

    public async Task<ApiResponse<List<BookingResponseDTO>>> GetMyBookingsAsync(int customerId)
    {
        var bookings = await _bookingRepository.GetByCustomerIdAsync(customerId);
        return ApiResponse<List<BookingResponseDTO>>.SuccessResponse(
            BookingMapper.ToDtoList(bookings),
            "Bookings fetched successfully");
    }

    public async Task<ApiResponse<List<BookingResponseDTO>>> GetBookingsByTripAsync(int tripId)
    {
        var bookings = await _bookingRepository.GetByTripIdAsync(tripId);
        return ApiResponse<List<BookingResponseDTO>>.SuccessResponse(
            BookingMapper.ToDtoList(bookings),
            "Bookings fetched successfully");
    }

    public async Task<ApiResponse<List<int>>> GetAvailableSeatsAsync(int tripId)
    {
        var seats = await _bookingRepository.GetAvailableSeatNumbersAsync(tripId);
        return ApiResponse<List<int>>.SuccessResponse(
            seats,
            "Available seats fetched successfully");
    }
}