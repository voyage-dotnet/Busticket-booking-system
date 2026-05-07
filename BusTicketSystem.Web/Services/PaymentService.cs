using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
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

    public async Task<PaymentResponseDTO> CreatePaymentAsync(PaymentRequestDTO request)
    {
        // Check booking exists and is Pending
        var booking = await _bookingRepository.GetByIdAsync(request.BookingId);

        if (booking == null)
            throw new NotFoundException("Booking not found");

        if (booking.Status != "Pending")
            throw new BadRequestException("Booking is not in Pending state");

        // Create payment
        var payment = new Payment
        {
            BookingId     = request.BookingId,
            CustomerId    = request.CustomerId,
            Amount        = request.Amount,
            PaymentDate   = DateTime.Now,
            PaymentStatus = "Success"
        };

        await _paymentRepository.AddAsync(payment);

        // Confirm the booking
        booking.Status = "Confirmed";
        await _bookingRepository.UpdateAsync(booking);

        return PaymentMapper.ToDto(payment);
    }

    public async Task<PaymentResponseDTO> GetPaymentByIdAsync(int paymentId)
    {
        var payment = await _paymentRepository.GetByIdAsync(paymentId);

        if (payment == null)
            throw new NotFoundException("Payment not found");

        return PaymentMapper.ToDto(payment);
    }

    public async Task<PaymentResponseDTO> GetPaymentByBookingIdAsync(int bookingId)
    {
        var payment = await _paymentRepository.GetByBookingIdAsync(bookingId);

        if (payment == null)
            throw new NotFoundException("No payment found for this booking");

        return PaymentMapper.ToDto(payment);
    }

    public async Task<List<PaymentResponseDTO>> GetMyPaymentsAsync(int customerId)
    {
        var payments = await _paymentRepository.GetByCustomerIdAsync(customerId);
        return PaymentMapper.ToDtoList(payments);
    }

    public async Task<object> GetAgencyRevenueAsync(int agencyId)
    {
        var revenue = await _paymentRepository.GetTotalRevenueByAgencyAsync(agencyId);
        return new { agencyId, totalRevenue = revenue };
    }

    public async Task<object> GetTripRevenueAsync(int tripId)
    {
        var revenue = await _paymentRepository.GetRevenueByTripAsync(tripId);
        var count   = await _paymentRepository.GetBookingCountByTripAsync(tripId);
        return new { tripId, totalRevenue = revenue, totalBookings = count };
    }
}