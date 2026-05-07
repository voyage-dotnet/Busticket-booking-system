using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BusTicketSystem.Web.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly BusTicketDbContext _context;

    public PaymentsController(BusTicketDbContext context)
    {
        _context = context;
    }

    // 1. POST /api/payments — Make a payment
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDto request)
    {
        // Check booking exists and is Pending
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.BookingId == request.BookingId
                                   && b.Status == "Pending");

        if (booking == null)
            return NotFound(new { message = "Booking not found or not in Pending state" });

        // Create payment record
        var payment = new Payment
        {
            BookingId     = request.BookingId,
            CustomerId    = request.CustomerId,
            Amount        = request.Amount,
            PaymentDate   = DateTime.Now,
            PaymentStatus = "Success"
        };

        _context.Payments.Add(payment);

        // Update booking to Confirmed
        booking.Status = "Confirmed";

        await _context.SaveChangesAsync();

        return Ok(new PaymentResponseDto
        {
            PaymentId     = payment.PaymentId,
            BookingId     = payment.BookingId,
            CustomerId    = payment.CustomerId,
            Amount        = payment.Amount,
            PaymentDate   = payment.PaymentDate,
            PaymentStatus = payment.PaymentStatus,
            Message       = "Payment successful. Booking confirmed."
        });
    }

    // 2. GET /api/payments/{id} — Get payment by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.PaymentId == id);

        if (payment == null)
            return NotFound(new { message = "Payment not found" });

        return Ok(new PaymentResponseDto
        {
            PaymentId     = payment.PaymentId,
            BookingId     = payment.BookingId,
            CustomerId    = payment.CustomerId,
            Amount        = payment.Amount,
            PaymentDate   = payment.PaymentDate,
            PaymentStatus = payment.PaymentStatus,
            Message       = "Payment fetched successfully"
        });
    }

    // 3. GET /api/payments/my?customerId={id} — Get all payments by a customer
    [HttpGet("my")]
    public async Task<IActionResult> GetMyPayments([FromQuery] int customerId)
    {
        var payments = await _context.Payments
            .Where(p => p.CustomerId == customerId)
            .Select(p => new PaymentResponseDto
            {
                PaymentId     = p.PaymentId,
                BookingId     = p.BookingId,
                CustomerId    = p.CustomerId,
                Amount        = p.Amount,
                PaymentDate   = p.PaymentDate,
                PaymentStatus = p.PaymentStatus,
                Message       = "OK"
            })
            .ToListAsync();

        return Ok(payments);
    }

    // 4. GET /api/payments/booking/{bookingId} — Get payment for a specific booking
    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetPaymentByBooking(int bookingId)
    {
        var payment = await _context.Payments
            .FirstOrDefaultAsync(p => p.BookingId == bookingId);

        if (payment == null)
            return NotFound(new { message = "No payment found for this booking" });

        return Ok(new PaymentResponseDto
        {
            PaymentId     = payment.PaymentId,
            BookingId     = payment.BookingId,
            CustomerId    = payment.CustomerId,
            Amount        = payment.Amount,
            PaymentDate   = payment.PaymentDate,
            PaymentStatus = payment.PaymentStatus,
            Message       = "Payment fetched successfully"
        });
    }
// 5. GET /api/payments/agency/revenue?agencyId={id} — Total revenue for agency
    [HttpGet("agency/revenue")]
    public async Task<IActionResult> GetAgencyRevenue([FromQuery] int agencyId)
    {
        var revenue = await _context.Payments
            .Where(p => p.PaymentStatus == "Success"
                     && _context.Bookings
                         .Any(b => b.BookingId == p.BookingId
                                && _context.Trips
                                    .Any(t => t.TripId == b.TripId
                                           && _context.Buses
                                               .Any(bus => bus.BusId == t.BusId
                                                        && _context.AgencyOffices
                                                            .Any(o => o.OfficeId == bus.OfficeId
                                                                   && o.AgencyId == agencyId)))))
            .SumAsync(p => p.Amount ?? 0);

        return Ok(new { agencyId, totalRevenue = revenue });
    }

    // 6. GET /api/payments/agency/trip/{tripId}/revenue — Revenue for a specific trip
    [HttpGet("agency/trip/{tripId}/revenue")]
    public async Task<IActionResult> GetTripRevenue(int tripId)
    {
        var revenue = await _context.Payments
            .Where(p => p.PaymentStatus == "Success"
                     && _context.Bookings
                         .Any(b => b.BookingId == p.BookingId
                                && b.TripId == tripId))
            .SumAsync(p => p.Amount ?? 0);

        var count = await _context.Payments
            .CountAsync(p => p.PaymentStatus == "Success"
                          && _context.Bookings
                              .Any(b => b.BookingId == p.BookingId
                                     && b.TripId == tripId));

        return Ok(new { tripId, totalRevenue = revenue, totalBookings = count });
    }
}