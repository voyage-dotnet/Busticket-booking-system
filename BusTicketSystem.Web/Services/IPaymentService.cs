using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services;

public interface IPaymentService
{
    Task<PaymentResponseDTO> CreatePaymentAsync(PaymentRequestDTO request);
    Task<PaymentResponseDTO> GetPaymentByIdAsync(int paymentId);
    Task<PaymentResponseDTO> GetPaymentByBookingIdAsync(int bookingId);
    Task<List<PaymentResponseDTO>> GetMyPaymentsAsync(int customerId);
    Task<object> GetAgencyRevenueAsync(int agencyId);
    Task<object> GetTripRevenueAsync(int tripId);
}