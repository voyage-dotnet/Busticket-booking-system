using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Repositories;

public interface IBookingRepository
{
    Task<Booking?> GetByIdAsync(int bookingId);
    Task<Booking> CreateAsync(Booking booking);
    Task<List<Booking>> GetByTripIdAsync(int tripId);
    Task<List<Booking>> GetByCustomerIdAsync(int customerId);
    Task<List<int>> GetBookedSeatNumbersAsync(int tripId);
    Task<List<int>> GetAvailableSeatNumbersAsync(int tripId);
    Task<bool> IsSeatAlreadyBookedAsync(int tripId, int seatNumber);
    Task<Trip?> GetTripByIdAsync(int tripId);
    Task UpdateAsync(Booking booking);
    Task<List<Booking>> GetPendingBookingsAsync();
}