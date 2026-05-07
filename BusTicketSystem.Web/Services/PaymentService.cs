using BusTicketSystem.Web.ApiResponse;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Mapping;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;

namespace BusTicketSystem.Web.Services;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBookingRepository _bookingRepository;

    public PaymentService(
        IPaymentRepository paymentRepository,
        IBookingRepository bookingRepository)
    {
        _paymentRepository = paymentRepository;
        _bookingRepository = bookingRepository;
    }

    public async Task<ApiResponse<PaymentResponseDTO>> CreatePaymentAsync(PaymentRequestDTO request)
    {
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

        if (booking == null)
            return ApiResponse<PaymentResponseDTO>.FailureResponse(
                "Booking not found", statusCode: 404);

        if (booking.Status != "Pending")
            return ApiResponse<PaymentResponseDTO>.FailureResponse(
                "Booking is not in Pending state", statusCode: 400);

        var payment = new Payment
        {
            BookingId     = request.BookingId,
            CustomerId    = request.CustomerId,
            Amount        = request.Amount,
            PaymentDate   = DateTime.Now,
            PaymentStatus = "Success"
        };

        await _paymentRepository.AddAsync(payment);
        booking.Status = "Confirmed";
        await _bookingRepository.UpdateAsync(booking);

        return ApiResponse<PaymentResponseDTO>.SuccessResponse(
            PaymentMapper.ToDto(payment),
            "Payment successful. Booking confirmed.");
    }

    public async Task<ApiResponse<PaymentResponseDTO>> GetPaymentByIdAsync(int paymentId)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);

        if (payment == null)
            return ApiResponse<PaymentResponseDTO>.FailureResponse(
                "Payment not found", statusCode: 404);

        return ApiResponse<PaymentResponseDTO>.SuccessResponse(
            PaymentMapper.ToDto(payment),
            "Payment fetched successfully");
    }

    public async Task<ApiResponse<PaymentResponseDTO>> GetPaymentByBookingIdAsync(int bookingId)
    {
        var payment = await _paymentRepository.GetByBookingIdAsync(bookingId);

        if (payment == null)
            return ApiResponse<PaymentResponseDTO>.FailureResponse(
                "No payment found for this booking", statusCode: 404);

        return ApiResponse<PaymentResponseDTO>.SuccessResponse(
            PaymentMapper.ToDto(payment),
            "Payment fetched successfully");
    }

    public async Task<ApiResponse<List<PaymentResponseDTO>>> GetMyPaymentsAsync(int customerId)
    {
        var payments = await _paymentRepository.GetByCustomerIdAsync(customerId);
        return ApiResponse<List<PaymentResponseDTO>>.SuccessResponse(
            PaymentMapper.ToDtoList(payments),
            "Payments fetched successfully");
    }

    public async Task<ApiResponse<object>> GetAgencyRevenueAsync(int agencyId)
    {
        var revenue = await _paymentRepository.GetTotalRevenueByAgencyAsync(agencyId);
        return ApiResponse<object>.SuccessResponse(
            new { agencyId, totalRevenue = revenue },
            "Revenue fetched successfully");
    }

    public async Task<ApiResponse<object>> GetTripRevenueAsync(int tripId)
    {
        var revenue = await _paymentRepository.GetRevenueByTripAsync(tripId);
        var count   = await _paymentRepository.GetBookingCountByTripAsync(tripId);
        return ApiResponse<object>.SuccessResponse(
            new { tripId, totalRevenue = revenue, totalBookings = count },
            "Trip revenue fetched successfully");
    }
}