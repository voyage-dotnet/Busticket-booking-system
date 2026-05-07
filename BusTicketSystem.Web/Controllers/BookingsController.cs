using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusTicketSystem.Web.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly BusTicketDbContext _context;

    public BookingsController(BusTicketDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto request)
    {
        var seat = await _context.Bookings
            .FirstOrDefaultAsync(b => b.TripId == request.TripId
                                   && b.SeatNumber == request.SeatNumber
                                   && b.Status == "Available");

        if (seat == null)
            return Conflict(new { message = "Seat is not available" });

        seat.Status = "Pending";
        await _context.SaveChangesAsync();

        return Ok(new BookingResponseDto
        {
            BookingId  = seat.BookingId,
            TripId     = seat.TripId ?? 0,
            SeatNumber = seat.SeatNumber,
            Status     = seat.Status,
            Message    = "Booking successful. Please complete payment within 10 minutes."
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.BookingId == id);

        if (booking == null)
            return NotFound(new { message = "Booking not found" });

        return Ok(new BookingResponseDto
        {
            BookingId  = booking.BookingId,
            TripId     = booking.TripId ?? 0,
            SeatNumber = booking.SeatNumber,
            Status     = booking.Status,
            Message    = "Booking fetched successfully"
        });
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyBookings([FromQuery] int customerId)
    {
        var bookings = await _context.Bookings
            .Where(b => _context.Payments
                .Any(p => p.BookingId == b.BookingId
                       && p.CustomerId == customerId))
            .Select(b => new BookingResponseDto
            {
                BookingId  = b.BookingId,
                TripId     = b.TripId ?? 0,
                SeatNumber = b.SeatNumber,
                Status     = b.Status,
                Message    = "OK"
            })
            .ToListAsync();

        return Ok(bookings);
    }

    [HttpGet("trip/{tripId}")]
    public async Task<IActionResult> GetBookingsByTrip(int tripId)
    {
        var bookings = await _context.Bookings
            .Where(b => b.TripId == tripId)
            .Select(b => new BookingResponseDto
            {
                BookingId  = b.BookingId,
                TripId     = b.TripId ?? 0,
                SeatNumber = b.SeatNumber,
                Status     = b.Status,
                Message    = "OK"
            })
            .ToListAsync();

        return Ok(bookings);
    }

    [HttpGet("available-seats/{tripId}")]
    public async Task<IActionResult> GetAvailableSeats(int tripId)
    {
        var seats = await _context.Bookings
            .Where(b => b.TripId == tripId && b.Status == "Available")
            .Select(b => b.SeatNumber)
            .ToListAsync();

        return Ok(new { tripId, availableSeats = seats });
    }
}