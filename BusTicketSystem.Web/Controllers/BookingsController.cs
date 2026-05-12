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
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDTO request)
    {
        var result = await _bookingService.CreateBookingAsync(GetCustomerId(), request);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetBookingById(int id)
    {
        var result = await _bookingService.GetBookingByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("my")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyBookings()
    {
        var result = await _bookingService.GetMyBookingsAsync(GetCustomerId());
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("trip/{tripId:int}")]
    [Authorize(Roles = "Agency")]
    public async Task<IActionResult> GetBookingsByTrip(int tripId)
    {
        var result = await _bookingService.GetBookingsByTripAsync(tripId);
        return StatusCode(result.StatusCode, result);
    }
    [HttpPut("{id:int}/cancel")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var result = await _bookingService.CancelBookingAsync(GetCustomerId(), id);
        return StatusCode(result.StatusCode, result);
    }
    [HttpGet("available-seats/{tripId:int}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetAvailableSeats(int tripId)
    {
        var result = await _bookingService.GetAvailableSeatsAsync(tripId);
        return StatusCode(result.StatusCode, result);
    }
}