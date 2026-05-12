using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.ResponseWrapper;
using BusTicketSystem.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusTicketSystem.Web.Controllers;

[ApiController]
[Route("api/payments")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _service;

    public PaymentsController(IPaymentService service)
    {
        _service = service;
    }

    private int GetCustomerId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private int GetAgencyId() =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    // ── POST /api/payments ───────────────────────────────────────────────────
    // Role: Customer — Process payment (UPI / CARD)
    // UPI   → returns QR code (base64) + UPI URL
    // CARD  → returns Success immediately
    [HttpPost]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestDTO dto)
    {
        try
        {
            var result = await _service.ProcessPaymentAsync(GetCustomerId(), dto);
            return StatusCode(201, ApiResponse<object>.SuccessResponse(
                result, "Payment processed successfully.", 201));
        }
        catch (BadRequestException ex)
        {
            return BadRequest(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 400));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 404));
        }
        catch (PaymentFailedException ex)
        {
            return StatusCode(402, ApiResponse<string>.FailureResponse(ex.Message, statusCode: 402));
        }
    }

    //// ── POST /api/payments/confirm/{bookingId} ───────────────────────────────
    //// Role: Customer — Confirm UPI payment after scanning QR
    //// (Same as Cruise confirm endpoint)
    //[HttpPost("confirm/{bookingId:int}")]
    //[Authorize(Roles = "Customer")]
    //public async Task<IActionResult> ConfirmUPIPayment(int bookingId)
    //{
    //    try
    //    {
    //        var result = await _service.ConfirmUPIPaymentAsync(bookingId);
    //        return Ok(ApiResponse<string>.SuccessResponse(result, result));
    //    }
    //    catch (NotFoundException ex)
    //    {
    //        return NotFound(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 404));
    //    }
    //    catch (BadRequestException ex)
    //    {
    //        return BadRequest(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 400));
    //    }
    //}

    // ── POST /api/payments/razorpay/create-order ─────────────────────────────
    // Role: Customer — Step 1: Create Razorpay order
    [HttpPost("razorpay/create-order")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> CreateRazorpayOrder([FromBody] CreateOrderDTO dto)
    {
        try
        {
            var result = await _service.CreateRazorpayOrderAsync(GetCustomerId(), dto);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Razorpay order created."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 404));
        }
    }

    // ── POST /api/payments/razorpay/verify ───────────────────────────────────
    // Role: Customer — Step 2: Verify Razorpay signature → sets booking Confirmed
    [HttpPost("razorpay/verify")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> VerifyRazorpayPayment([FromBody] VerifyPaymentDTO dto)
    {
        try
        {
            var result = await _service.VerifyAndSavePaymentAsync(dto);
            return Ok(ApiResponse<object>.SuccessResponse(result, "Payment verified successfully."));
        }
        catch (PaymentFailedException ex)
        {
            return StatusCode(402, ApiResponse<string>.FailureResponse(ex.Message, statusCode: 402));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 404));
        }
    }

    // ── GET /api/payments/{id} ───────────────────────────────────────────────
    // Role: Customer/Agency — Payment details by ID
    [HttpGet("{id:int}")]
    [Authorize]
    public async Task<IActionResult> GetPaymentById(int id)
    {
        try
        {
            var payment = await _service.GetPaymentByIdAsync(GetCustomerId(), id);
            return Ok(ApiResponse<PaymentResponseDTO>.SuccessResponse(
                payment!, "Payment fetched successfully."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 404));
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, ApiResponse<string>.FailureResponse(ex.Message, statusCode: 403));
        }
    }

    // ── GET /api/payments/booking/{bookingId} ────────────────────────────────
    // Role: Customer/Agency — Get payment for specific booking
    [HttpGet("booking/{bookingId:int}")]
    [Authorize]
    public async Task<IActionResult> GetPaymentByBookingId(int bookingId)
    {
        try
        {
            var payment = await _service.GetPaymentByBookingIdAsync(GetCustomerId(), bookingId);
            return Ok(ApiResponse<PaymentResponseDTO>.SuccessResponse(
                payment!, "Payment fetched successfully."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 404));
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, ApiResponse<string>.FailureResponse(ex.Message, statusCode: 403));
        }
    }

    // ── GET /api/payments/my ─────────────────────────────────────────────────
    // Role: Customer — Own full payment history
    [HttpGet("my")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyPaymentHistory()
    {
        var history = await _service.GetMyPaymentHistoryAsync(GetCustomerId());
        return Ok(ApiResponse<List<PaymentHistoryDTO>>.SuccessResponse(
            history, "Payment history fetched successfully."));
    }

    // ── GET /api/payments/agency/revenue ─────────────────────────────────────
    // Role: Agency — Total revenue from all trips
    [HttpGet("agency/revenue")]
    [Authorize(Roles = "Agency")]
    public async Task<IActionResult> GetAgencyRevenue()
    {
        var revenue = await _service.GetAgencyRevenueAsync(GetAgencyId());
        return Ok(ApiResponse<AgencyRevenueDTO>.SuccessResponse(
            revenue, "Agency revenue fetched successfully."));
    }

    // ── GET /api/payments/agency/trip/{tripId}/revenue ───────────────────────
    // Role: Agency — Revenue breakdown per trip
    [HttpGet("agency/trip/{tripId:int}/revenue")]
    [Authorize(Roles = "Agency")]
    public async Task<IActionResult> GetTripRevenue(int tripId)
    {
        try
        {
            var revenue = await _service.GetTripRevenueAsync(GetAgencyId(), tripId);
            return Ok(ApiResponse<TripRevenueDTO>.SuccessResponse(
                revenue, "Trip revenue fetched successfully."));
        }
        catch (NotFoundException ex)
        {
            return NotFound(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 404));
        }
        catch (ForbiddenException ex)
        {
            return StatusCode(403, ApiResponse<string>.FailureResponse(ex.Message, statusCode: 403));
        }
    }
}