namespace BusTicketSystem.Web.DTOs;

public class BookingRequestDTO
{
    public int TripId { get; set; }
    public int SeatNumber { get; set; }
    public int CustomerId { get; set; }
}

public class BookingResponseDTO
{
    public int BookingId { get; set; }
    public int TripId { get; set; }
    public int SeatNumber { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class PaymentRequestDTO
{
    public int BookingId { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
}

public class PaymentResponseDTO
{
    public int PaymentId { get; set; }
    public int BookingId { get; set; }
    public int? CustomerId { get; set; }
    public decimal? Amount { get; set; }
    public DateTime? PaymentDate { get; set; }
    public string? PaymentStatus { get; set; }
}

