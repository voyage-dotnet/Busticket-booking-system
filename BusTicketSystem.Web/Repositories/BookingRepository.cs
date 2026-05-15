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
            .Include(b => b.Trip)
                .ThenInclude(t => t!.Route)
            .Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId);
    }
    public async Task<Booking> CreateAsync(Booking booking)
    {
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<List<Booking>> GetByTripIdAsync(int tripId)
    {
        return await _context.Bookings
            .Where(b => b.TripId == tripId)
            .Include(b => b.Trip)
                .ThenInclude(t => t!.Route)
            .ToListAsync();
    }
    public async Task<List<Booking>> GetByCustomerIdAsync(int customerId)
    {
        return await _context.Bookings
            .Where(b => _context.Payments
                .Any(p => p.BookingId == b.BookingId
                       && p.CustomerId == customerId))
            .Include(b => b.Trip)
                .ThenInclude(t => t!.Route)
            .ToListAsync();
    }
    public async Task<List<int>> GetBookedSeatNumbersAsync(int tripId)
    {
        return await _context.Bookings
            .Where(b => b.TripId == tripId
                     && b.Status != "Cancelled")
            .Select(b => b.SeatNumber)
            .ToListAsync();
    }
    public async Task<List<int>> GetAvailableSeatNumbersAsync(int tripId)
    {
        var trip = await _context.Trips
            .Include(t => t.Bus)
            .FirstOrDefaultAsync(t => t.TripId == tripId);

        if (trip == null) return new List<int>();

        var totalSeats = trip.Bus.Capacity;
        var bookedSeats = await GetBookedSeatNumbersAsync(tripId);

        return Enumerable.Range(1, totalSeats)
            .Where(s => !bookedSeats.Contains(s))
            .ToList();
    }
    public async Task<bool> IsSeatAlreadyBookedAsync(int tripId, int seatNumber)
    {
        return await _context.Bookings
            .AnyAsync(b => b.TripId == tripId
                        && b.SeatNumber == seatNumber
                        && b.Status != "Cancelled");
    }

    public async Task<Trip?> GetTripByIdAsync(int tripId)
    {
        return await _context.Trips
            .Include(t => t.Route)
            .Include(t => t.Bus)
            .FirstOrDefaultAsync(t => t.TripId == tripId);
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }

    public async Task<List<Booking>> GetPendingBookingsAsync()
    {
        return await _context.Bookings
            .Where(b => b.Status == "Pending"
                     && !_context.Payments
                         .Any(p => p.BookingId == b.BookingId
                                && p.PaymentStatus == "Success"))
            .ToListAsync();
    }
}
