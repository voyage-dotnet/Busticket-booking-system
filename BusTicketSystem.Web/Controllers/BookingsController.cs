using BusTicketSystem.Web.ApiResponse;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Services;
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

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDTO request)
    {
        var result = await _bookingService.CreateBookingAsync(request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        var result = await _bookingService.GetBookingByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings([FromQuery] int customerId)
    {
        var result = await _bookingService.GetMyBookingsAsync(customerId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("trip/{tripId}")]
    public async Task<IActionResult> GetBookingsByTrip(int tripId)
    {
        var result = await _bookingService.GetBookingsByTripAsync(tripId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("available-seats/{tripId}")]
    public async Task<IActionResult> GetAvailableSeats(int tripId)
    {
        var result = await _bookingService.GetAvailableSeatsAsync(tripId);
        return StatusCode(result.StatusCode, result);
    }
}