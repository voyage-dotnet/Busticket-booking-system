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

    // ── Read-only Booking (needed to validate before payment) ─────────────────
    Task<Booking?> GetBookingByIdAsync(int bookingId);
    Task UpdateBookingStatusAsync(int bookingId, string status);
    Task<bool> IsSeatAlreadyBookedAsync(int tripId, int seatNumber);

    // ── Read-only Trip (needed for fare validation) ───────────────────────────
    Task<Trip?> GetTripByIdAsync(int tripId);
}