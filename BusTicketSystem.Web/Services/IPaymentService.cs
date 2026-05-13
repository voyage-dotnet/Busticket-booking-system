using BusTicketSystem.Web.ResponseWrapper;
using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services;

public interface IPaymentService
{
    Task<object> ProcessPaymentAsync(int customerId, PaymentRequestDTO dto);
    Task<string> ConfirmUPIPaymentAsync(int bookingId);
    Task<object> CreateRazorpayOrderAsync(int customerId, CreateOrderDTO dto);
    Task<object> VerifyAndSavePaymentAsync(VerifyPaymentDTO dto);
    Task<PaymentResponseDTO?> GetPaymentByIdAsync(int customerId, int paymentId);
    Task<PaymentResponseDTO?> GetPaymentByBookingIdAsync(int customerId, int bookingId);
    Task<List<PaymentHistoryDTO>> GetMyPaymentHistoryAsync(int customerId);
    Task<AgencyRevenueDTO> GetAgencyRevenueAsync(int agencyId);
    Task<TripRevenueDTO> GetTripRevenueAsync(int agencyId, int tripId);
}