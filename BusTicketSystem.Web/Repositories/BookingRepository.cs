using BusTicketSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BusTicketSystem.Web.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly BusTicketDbContext _context;

    public BookingRepository(BusTicketDbContext context)
    {
        _context = context;
    }

    public async Task<Booking?> GetByIdAsync(int bookingId)
    {
        return await _context.Bookings
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);
    }

    public async Task<Booking?> GetAvailableSeatAsync(int tripId, int seatNumber)
    {
        return await _context.Bookings
            .FirstOrDefaultAsync(b => b.TripId == tripId
                                   && b.SeatNumber == seatNumber
                                   && b.Status == "Available");
    }

    public async Task<List<Booking>> GetByTripIdAsync(int tripId)
    {
        return await _context.Bookings
            .Where(b => b.TripId == tripId)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Bookings
            .Where(b => _context.Payments
                .Any(p => p.BookingId == b.BookingId
                       && p.CustomerId == customerId))
            .ToListAsync();
    }

    public async Task<List<int>> GetAvailableSeatNumbersAsync(int tripId)
    {
        return await _context.Bookings
            .Where(b => b.TripId == tripId && b.Status == "Available")
            .Select(b => b.SeatNumber)
            .ToListAsync();
    }

    public async Task<List<Booking>> GetPendingBookingsAsync()
    {
        return await _context.Bookings
            .Where(b => b.Status == "Pending"
                     && !_context.Payments
                         .Any(p => p.BookingId == b.BookingId))
            .ToListAsync();
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }
}