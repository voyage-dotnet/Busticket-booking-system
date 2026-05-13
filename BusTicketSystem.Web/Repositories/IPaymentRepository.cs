using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Repositories;

public interface IPaymentRepository
{
    Task AddAsync(Payment payment);
    Task<Payment?> GetByIdAsync(int paymentId);
    Task<Payment?> GetByBookingIdAsync(int bookingId);
    Task<List<Payment>> GetAllByCustomerIdAsync(int customerId);
    Task<List<Payment>> GetAllByAgencyAsync(int agencyId);
    Task<List<Payment>> GetAllByTripAsync(int tripId);
    Task UpdateAsync(Payment payment);
    Task<Booking?> GetBookingByIdAsync(int bookingId);
    Task UpdateBookingStatusAsync(int bookingId, string status);
    Task<bool> IsSeatAlreadyBookedAsync(int tripId, int seatNumber);
    Task<Trip?> GetTripByIdAsync(int tripId);
}