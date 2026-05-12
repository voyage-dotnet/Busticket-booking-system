using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

    private int GetCustomerId() => 
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // POST /api/bookings
    // Role: Customer — Book a seat → status = 'Pending'
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDTO request)
    {
        var result = await _bookingService.CreateBookingAsync(GetCustomerId(), request);
        return StatusCode(result.StatusCode, result);
    }

    // GET /api/bookings/{id}
    // Role: Customer/Agency — Get booking details
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetBookingById(int id)
    {
        var result = await _bookingService.GetBookingByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    // GET /api/bookings/my
    // Role: Customer — Own booking history
    [HttpGet("my")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyBookings()
    {
        var result = await _bookingService.GetMyBookingsAsync(GetCustomerId());
        return StatusCode(result.StatusCode, result);
    }

    // GET /api/bookings/trip/{tripId}
    // Role: Agency — All bookings for a trip (agency filtered)
    [HttpGet("trip/{tripId:int}")]
    [Authorize(Roles = "Agency")]
    public async Task<IActionResult> GetBookingsByTrip(int tripId)
    {
        var result = await _bookingService.GetBookingsByTripAsync(tripId);
        return StatusCode(result.StatusCode, result);
    }

    // PUT /api/bookings/{id}/cancel
    // Role: Customer — Cancel booking → seat becomes Available
    [HttpPut("{id:int}/cancel")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var result = await _bookingService.CancelBookingAsync(GetCustomerId(), id);
        return StatusCode(result.StatusCode, result);
    }

    // GET /api/bookings/available-seats/{tripId}
    // Role: Public — Available seat numbers list
    [HttpGet("available-seats/{tripId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailableSeats(int tripId)
    {
        var result = await _bookingService.GetAvailableSeatsAsync(tripId);
        return StatusCode(result.StatusCode, result);
    }
}