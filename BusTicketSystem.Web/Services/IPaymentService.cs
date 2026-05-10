using BusTicketSystem.Web.ApiResponse;
using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services;

public interface IPaymentService
{
    Task<ApiResponse<PaymentResponseDTO>> CreatePaymentAsync(PaymentRequestDTO request);
    Task<ApiResponse<PaymentResponseDTO>> GetPaymentByIdAsync(int paymentId);
    Task<ApiResponse<PaymentResponseDTO>> GetPaymentByBookingIdAsync(int bookingId);
    Task<ApiResponse<List<PaymentResponseDTO>>> GetMyPaymentsAsync(int customerId);
    Task<ApiResponse<object>> GetAgencyRevenueAsync(int agencyId);
    Task<ApiResponse<object>> GetTripRevenueAsync(int tripId);
}