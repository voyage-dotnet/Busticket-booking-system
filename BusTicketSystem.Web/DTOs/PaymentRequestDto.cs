namespace BusTicketSystem.Web.DTOs;

public class PaymentRequestDto
{
    public int BookingId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
}