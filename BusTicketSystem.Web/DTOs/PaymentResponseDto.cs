namespace BusTicketSystem.Web.DTOs;

public class PaymentResponseDto
{
    public int PaymentId { get; set; }
    public int BookingId { get; set; }
    public int? CustomerId { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? PaymentStatus { get; set; }
    public string Message { get; set; } = string.Empty;
}