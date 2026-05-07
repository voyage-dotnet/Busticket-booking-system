namespace BusTicketSystem.Web.DTOs;

public class BookingResponseDto
{
    public int BookingId { get; set; }
    public int TripId { get; set; }
    public int SeatNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}