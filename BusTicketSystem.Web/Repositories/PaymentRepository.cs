using BusTicketSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BusTicketSystem.Web.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly BusTicketDbContext _db;

    public PaymentRepository(BusTicketDbContext db)
    {
        _db = db;
    }

    public async Task AddAsync(Payment payment)
    {
        _db.Payments.Add(payment);
        await _db.SaveChangesAsync();
    }

    public async Task<Payment?> GetByIdAsync(int paymentId)
    {
        return await _db.Payments
            .Include(p => p.Booking)
                .ThenInclude(b => b.Trip)
                    .ThenInclude(t => t!.Route)
            .Include(p => p.Customer)
            .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
    }

    public async Task<Payment?> GetByBookingIdAsync(int bookingId)
    {
        return await _db.Payments
            .Include(p => p.Booking)
                .ThenInclude(b => b.Trip)
                    .ThenInclude(t => t!.Route)
            .FirstOrDefaultAsync(p => p.BookingId == bookingId);
    }

    public async Task<List<Payment>> GetAllByCustomerIdAsync(int customerId)
    {
        return await _db.Payments
            .Where(p => p.CustomerId == customerId)
            .Include(p => p.Booking)
                .ThenInclude(b => b.Trip)
                    .ThenInclude(t => t!.Route)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }
    public async Task<List<Payment>> GetAllByAgencyAsync(int agencyId)
    {
        return await _db.Payments
            .Where(p => p.PaymentStatus == "Success"
                     && p.Booking.Trip!.Bus.Office.AgencyId == agencyId)
            .Include(p => p.Booking)
                .ThenInclude(b => b.Trip)
                    .ThenInclude(t => t!.Route)
            .Include(p => p.Booking)
                .ThenInclude(b => b.Trip)
                    .ThenInclude(t => t!.Bus)
                        .ThenInclude(b => b.Office)
                            .ThenInclude(o => o.Agency)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task<List<Payment>> GetAllByTripAsync(int tripId)
    {
        return await _db.Payments
            .Where(p => p.PaymentStatus == "Success"
                     && p.Booking.TripId == tripId)
            .Include(p => p.Booking)
                .ThenInclude(b => b.Trip)
                    .ThenInclude(t => t!.Route)
            .Include(p => p.Customer)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync();
    }

    public async Task UpdateAsync(Payment payment)
    {
        _db.Payments.Update(payment);
        await _db.SaveChangesAsync();
    }

    public async Task<Booking?> GetBookingByIdAsync(int bookingId)
    {
        return await _db.Bookings
            .Include(b => b.Trip)
                .ThenInclude(t => t!.Route)
            .Include(b => b.Trip)
                .ThenInclude(t => t!.Bus)
            .Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);
    }

    public async Task UpdateBookingStatusAsync(int bookingId, string status)
    {
        var booking = await _db.Bookings.FindAsync(bookingId);
        if (booking != null)
        {
            booking.Status = status;
            await _db.SaveChangesAsync();
        }
    }

    public async Task<bool> IsSeatAlreadyBookedAsync(int tripId, int seatNumber)
    {
        return await _db.Bookings
            .AnyAsync(b => b.TripId == tripId
                        && b.SeatNumber == seatNumber
                        && b.Status == "Booked");
    }

    public async Task<Trip?> GetTripByIdAsync(int tripId)
    {
        return await _db.Trips
            .Include(t => t.Route)
            .Include(t => t.Bus)
                .ThenInclude(b => b.Office)
                    .ThenInclude(o => o.Agency)
            .FirstOrDefaultAsync(t => t.TripId == tripId);
    }
}