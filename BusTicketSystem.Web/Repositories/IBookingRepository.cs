using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int bookingId);
    Task<Booking?> GetAvailableSeatAsync(int tripId, int seatNumber);
    Task<List<Booking>> GetByTripIdAsync(int tripId);
    Task<List<Booking>> GetByCustomerIdAsync(int customerId);
    Task<List<int>> GetAvailableSeatNumbersAsync(int tripId);
    Task<List<Booking>> GetPendingBookingsAsync();
    Task UpdateAsync(Booking booking);
}