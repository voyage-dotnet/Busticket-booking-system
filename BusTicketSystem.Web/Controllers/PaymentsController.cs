using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Services;
using BusTicketSystem.Web.Wrapper;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.Web.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // 1. POST /api/payments — Make a payment
    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDTO request)
    {
        var result = await _paymentService.CreatePaymentAsync(request);
        return Ok(ApiResponse<PaymentResponseDTO>.SuccessResponse(result,
            "Payment successful. Booking confirmed."));
    }

    // 2. GET /api/payments/{id} — Get payment by ID
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        var result = await _paymentService.GetPaymentByIdAsync(id);
        return Ok(ApiResponse<PaymentResponseDTO>.SuccessResponse(result,
            "Payment fetched successfully"));
    }

    // 3. GET /api/payments/my?customerId={id} — Get all payments by a customer
    [HttpGet("my")]
    public async Task<IActionResult> GetMyPayments([FromQuery] int customerId)
    {
        var result = await _paymentService.GetMyPaymentsAsync(customerId);
        return Ok(ApiResponse<List<PaymentResponseDTO>>.SuccessResponse(result,
            "Payments fetched successfully"));
    }

    // 4. GET /api/payments/booking/{bookingId} — Get payment for a specific booking
    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetPaymentByBooking(int bookingId)
    {
        var result = await _paymentService.GetPaymentByBookingIdAsync(bookingId);
        return Ok(ApiResponse<PaymentResponseDTO>.SuccessResponse(result,
            "Payment fetched successfully"));
    }

    // 5. GET /api/payments/agency/revenue?agencyId={id} — Total revenue for agency
    [HttpGet("agency/revenue")]
    public async Task<IActionResult> GetAgencyRevenue([FromQuery] int agencyId)
    {
        var result = await _paymentService.GetAgencyRevenueAsync(agencyId);
        return Ok(ApiResponse<object>.SuccessResponse(result,
            "Revenue fetched successfully"));
    }

    // 6. GET /api/payments/agency/trip/{tripId}/revenue — Revenue for a specific trip
    [HttpGet("agency/trip/{tripId}/revenue")]
    public async Task<IActionResult> GetTripRevenue(int tripId)
    {
        var result = await _paymentService.GetTripRevenueAsync(tripId);
        return Ok(ApiResponse<object>.SuccessResponse(result,
            "Trip revenue fetched successfully"));
    }
}