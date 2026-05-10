using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Mapping;

public static class PaymentMapper
{
    public static PaymentResponseDTO ToDto(Payment payment)
    {
        return new PaymentResponseDTO
        {
            PaymentId     = payment.PaymentId,
            BookingId     = payment.BookingId,
            CustomerId    = payment.CustomerId,
            Amount        = payment.Amount,
            PaymentDate   = payment.PaymentDate,
            PaymentStatus = payment.PaymentStatus
        };
    }

    public static List<PaymentResponseDTO> ToDtoList(List<Payment> payments)
    {
        return payments.Select(ToDto).ToList();
    }
}