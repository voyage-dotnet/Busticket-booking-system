using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Helper;
using BusTicketSystem.Web.Mapping;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;
using BusTicketSystem.Web.ResponseWrapper;
using BusTicketSystem.Web.Validator;

namespace BusTicketSystem.Web.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;

    public BookingService(IBookingRepository bookingRepo)
    {
        _bookingRepo = bookingRepo;
    }

    // ─── POST /api/bookings ───────────────────────────────────────────────────

    public async Task<ApiResponse<BookingResponseDTO>> CreateBookingAsync(
        int customerId, BookingRequestDTO request)
    {
        // 1. Validate input using validator
        var errors = BookingValidator.Validate(request);
        if (errors.Any())
            return ApiResponse<BookingResponseDTO>.FailureResponse(
                string.Join(" | ", errors), statusCode: 400);

        // 2. Check trip exists
        var trip = await _bookingRepo.GetTripByIdAsync(request.TripId);
        if (trip == null)
            return ApiResponse<BookingResponseDTO>.FailureResponse(
                "Trip not found.", statusCode: 404);

        //// 3. Check available seats
        //if (trip.AvailableSeats <= 0)
        //    return ApiResponse<BookingResponseDTO>.FailureResponse(
        //        "No available seats on this trip.", statusCode: 409);

        // 3. Validate seat number within bus capacity
        if (request.SeatNumber < 1 || request.SeatNumber > trip.Bus.Capacity)
            return ApiResponse<BookingResponseDTO>.FailureResponse(
                $"Seat number must be between 1 and {trip.Bus.Capacity}.", statusCode: 400);

        // 5. Check seat not already taken
        var isTaken = await _bookingRepo.IsSeatAlreadyBookedAsync(
            request.TripId, request.SeatNumber);
        if (isTaken)
            return ApiResponse<BookingResponseDTO>.FailureResponse(
                $"Seat {request.SeatNumber} is already booked.", statusCode: 409);

        // 6. Create booking with Status = "Pending"
        var booking = new Booking
        {
            TripId = request.TripId,
            SeatNumber = request.SeatNumber,
            Status = "Booked"
        };

        var created = await _bookingRepo.CreateAsync(booking);

        // 7. Track for auto-expire after 10 minutes
        BookingTimeoutHelper.TrackBooking(created.BookingId);

        // 8. Re-fetch with navigations for mapper
        var saved = await _bookingRepo.GetByIdAsync(created.BookingId);

        return ApiResponse<BookingResponseDTO>.SuccessResponse(
            BookingMapper.ToDto(saved!),
            "Booking created. Please complete payment within 10 minutes.",
            201);
    }

    // ─── GET /api/bookings/{id} ───────────────────────────────────────────────

    public async Task<ApiResponse<BookingResponseDTO>> GetBookingByIdAsync(int bookingId)
    {
        var booking = await _bookingRepo.GetByIdAsync(bookingId);
        if (booking == null)
            return ApiResponse<BookingResponseDTO>.FailureResponse(
                "Booking not found.", statusCode: 404);

        return ApiResponse<BookingResponseDTO>.SuccessResponse(
            BookingMapper.ToDto(booking),
            "Booking fetched successfully.");
    }

    // ─── GET /api/bookings/my ─────────────────────────────────────────────────

    public async Task<ApiResponse<List<BookingResponseDTO>>> GetMyBookingsAsync(int customerId)
    {
        var bookings = await _bookingRepo.GetByCustomerIdAsync(customerId);
        return ApiResponse<List<BookingResponseDTO>>.SuccessResponse(
            BookingMapper.ToDtoList(bookings),
            "Bookings fetched successfully.");
    }

    // ─── GET /api/bookings/trip/{tripId} ──────────────────────────────────────

    public async Task<ApiResponse<List<BookingResponseDTO>>> GetBookingsByTripAsync(int tripId)
    {
        var bookings = await _bookingRepo.GetByTripIdAsync(tripId);
        return ApiResponse<List<BookingResponseDTO>>.SuccessResponse(
            BookingMapper.ToDtoList(bookings),
            "Bookings fetched successfully.");
    }

    // ─── GET /api/bookings/available-seats/{tripId} ───────────────────────────

    public async Task<ApiResponse<List<int>>> GetAvailableSeatsAsync(int tripId)
    {
        var trip = await _bookingRepo.GetTripByIdAsync(tripId);
        if (trip == null)
            return ApiResponse<List<int>>.FailureResponse(
                "Trip not found.", statusCode: 404);

        var seats = await _bookingRepo.GetAvailableSeatNumbersAsync(tripId);
        return ApiResponse<List<int>>.SuccessResponse(
            seats,
            $"{seats.Count} seat(s) available.");
    }

    // ─── PUT /api/bookings/{id}/cancel ────────────────────────────────────────

    public async Task<ApiResponse<string>> CancelBookingAsync(int customerId, int bookingId)
    {
        var booking = await _bookingRepo.GetByIdAsync(bookingId);
        if (booking == null)
            return ApiResponse<string>.FailureResponse(
                "Booking not found.", statusCode: 404);

        // Verify ownership via payment record
        var isOwner = booking.Payments.Any(p => p.CustomerId == customerId);
        if (!isOwner)
            return ApiResponse<string>.FailureResponse(
                "You are not authorized to cancel this booking.", statusCode: 403);

        if (booking.Status == "Cancelled")
            return ApiResponse<string>.FailureResponse(
                "Booking is already cancelled.", statusCode: 400);

        if (booking.Status == "Confirmed")
            return ApiResponse<string>.FailureResponse(
                "Confirmed bookings cannot be cancelled.", statusCode: 400);

        booking.Status = "Cancelled";
        await _bookingRepo.UpdateAsync(booking);

        return ApiResponse<string>.SuccessResponse(
            "Booking cancelled successfully.",
            "Booking cancelled. Seat is now available.");
    }
}