using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Repositories;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(int paymentId);
    Task<Payment?> GetByBookingIdAsync(int bookingId);
    Task<List<Payment>> GetByCustomerIdAsync(int customerId);
    Task<decimal> GetTotalRevenueByAgencyAsync(int agencyId);
    Task<decimal> GetRevenueByTripAsync(int tripId);
    Task<int> GetBookingCountByTripAsync(int tripId);
    Task AddAsync(Payment payment);
}