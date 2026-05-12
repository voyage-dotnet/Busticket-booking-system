using BusTicketSystem.Web.ResponseWrapper;
using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services;

public interface IPaymentService
{
    
    // ── Core (matching Cruise pattern) ────────────────────────────────────────
    Task<object> ProcessPaymentAsync(int customerId, PaymentRequestDTO dto);
    Task<string> ConfirmUPIPaymentAsync(int bookingId);

    // ── Razorpay (matching Cruise pattern) ───────────────────────────────────
    Task<object> CreateRazorpayOrderAsync(int customerId, CreateOrderDTO dto);
    Task<object> VerifyAndSavePaymentAsync(VerifyPaymentDTO dto);

    // ── GET endpoints ─────────────────────────────────────────────────────────
    Task<PaymentResponseDTO?> GetPaymentByIdAsync(int customerId, int paymentId);
    Task<PaymentResponseDTO?> GetPaymentByBookingIdAsync(int customerId, int bookingId);
    Task<List<PaymentHistoryDTO>> GetMyPaymentHistoryAsync(int customerId);

    // ── Agency Revenue ────────────────────────────────────────────────────────
    Task<AgencyRevenueDTO> GetAgencyRevenueAsync(int agencyId);
    Task<TripRevenueDTO> GetTripRevenueAsync(int agencyId, int tripId);
}