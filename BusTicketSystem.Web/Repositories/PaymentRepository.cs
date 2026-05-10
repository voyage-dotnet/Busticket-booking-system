using BusTicketSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BusTicketSystem.Web.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly BusTicketDbContext _context;

    public PaymentRepository(BusTicketDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(int paymentId)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
    }

    public async Task<Payment?> GetByBookingIdAsync(int bookingId)
    {
        return await _context.Payments
            .FirstOrDefaultAsync(p => p.BookingId == bookingId);
    }

    public async Task<List<Payment>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Payments
            .Where(p => p.CustomerId == customerId)
            .ToListAsync();
    }

    public async Task<decimal> GetTotalRevenueByAgencyAsync(int agencyId)
    {
        return await _context.Payments
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
    }

    public async Task<decimal> GetRevenueByTripAsync(int tripId)
    {
        return await _context.Payments
            .Where(p => p.PaymentStatus == "Success"
                     && _context.Bookings
                         .Any(b => b.BookingId == p.BookingId
                                && b.TripId == tripId))
            .SumAsync(p => p.Amount ?? 0);
    }

    public async Task<int> GetBookingCountByTripAsync(int tripId)
    {
        return await _context.Payments
            .CountAsync(p => p.PaymentStatus == "Success"
                          && _context.Bookings
                              .Any(b => b.BookingId == p.BookingId
                                     && b.TripId == tripId));
    }

    public async Task AddAsync(Payment payment)
    {
        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();
    }
}