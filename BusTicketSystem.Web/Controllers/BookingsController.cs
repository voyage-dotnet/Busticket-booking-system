using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Services;
using BusTicketSystem.Web.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.Web.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    // 1. POST /api/bookings — Book a seat
    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDTO request)
    {
        var result = await _bookingService.CreateBookingAsync(request);
        return Ok(ApiResponse<BookingResponseDTO>.SuccessResponse(result,
            "Booking successful. Please complete payment within 10 minutes."));
    }

    // 2. GET /api/bookings/{id} — Get booking by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        var result = await _bookingService.GetBookingByIdAsync(id);
        return Ok(ApiResponse<BookingResponseDTO>.SuccessResponse(result,
            "Booking fetched successfully"));
    }

    // 3. GET /api/bookings/my?customerId={id} — Get customer's own bookings
    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings([FromQuery] int customerId)
    {
        var result = await _bookingService.GetMyBookingsAsync(customerId);
        return Ok(ApiResponse<List<BookingResponseDTO>>.SuccessResponse(result,
            "Bookings fetched successfully"));
    }

    // 4. GET /api/bookings/trip/{tripId} — All bookings for a trip
    [HttpGet("trip/{tripId}")]
    public async Task<IActionResult> GetBookingsByTrip(int tripId)
    {
        var result = await _bookingService.GetBookingsByTripAsync(tripId);
        return Ok(ApiResponse<List<BookingResponseDTO>>.SuccessResponse(result,
            "Bookings fetched successfully"));
    }

    // 5. GET /api/bookings/available-seats/{tripId} — Get available seats
    [HttpGet("available-seats/{tripId}")]
    public async Task<IActionResult> GetAvailableSeats(int tripId)
    {
        var result = await _bookingService.GetAvailableSeatsAsync(tripId);
        return Ok(ApiResponse<object>.SuccessResponse(
            new { tripId, availableSeats = result },
            "Available seats fetched successfully"));
    }
}