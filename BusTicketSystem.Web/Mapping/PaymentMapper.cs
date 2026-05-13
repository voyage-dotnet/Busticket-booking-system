using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Mapping;

public static class PaymentMapper
{
   
    public static PaymentResponseDTO ToPaymentResponseDTO(
        Payment p,
        string? qrCode = null,
        string? upiUrl = null)
    {
        return new PaymentResponseDTO
        {
            PaymentId = p.PaymentId,
            BookingId = p.BookingId,
            RouteName = $"{p.Booking?.Trip?.Route?.FromCity} -> {p.Booking?.Trip?.Route?.ToCity}",
            Amount = p.Amount ?? 0m,
            PaymentDate = p.PaymentDate ?? DateTime.UtcNow,
            PaymentStatus = p.PaymentStatus ?? string.Empty,
            QRCode = qrCode,   
            UpiUrl = upiUrl    
        };
    }
    public static PaymentHistoryDTO ToPaymentHistoryDTO(Payment p)
    {
        return new PaymentHistoryDTO
        {
            PaymentId = p.PaymentId,
            BookingId = p.BookingId,
            RouteName = $"{p.Booking?.Trip?.Route?.FromCity} -> {p.Booking?.Trip?.Route?.ToCity}",
            Amount = p.Amount ?? 0m,
            PaymentDate = p.PaymentDate ?? DateTime.UtcNow,
            PaymentStatus = p.PaymentStatus ?? string.Empty,
            BookingStatus = p.Booking?.Status ?? string.Empty
        };
    }
}