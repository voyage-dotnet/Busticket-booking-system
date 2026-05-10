using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Services;
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

    [HttpPost]
    public async Task<IActionResult> CreatePayment([FromBody] PaymentRequestDTO request)
    {
        var result = await _paymentService.CreatePaymentAsync(request);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        var result = await _paymentService.GetPaymentByIdAsync(id);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyPayments([FromQuery] int customerId)
    {
        var result = await _paymentService.GetMyPaymentsAsync(customerId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("booking/{bookingId}")]
    public async Task<IActionResult> GetPaymentByBooking(int bookingId)
    {
        var result = await _paymentService.GetPaymentByBookingIdAsync(bookingId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("agency/revenue")]
    public async Task<IActionResult> GetAgencyRevenue([FromQuery] int agencyId)
    {
        var result = await _paymentService.GetAgencyRevenueAsync(agencyId);
        return StatusCode(result.StatusCode, result);
    }

    [HttpGet("agency/trip/{tripId}/revenue")]
    public async Task<IActionResult> GetTripRevenue(int tripId)
    {
        var result = await _paymentService.GetTripRevenueAsync(tripId);
        return StatusCode(result.StatusCode, result);
    }
}