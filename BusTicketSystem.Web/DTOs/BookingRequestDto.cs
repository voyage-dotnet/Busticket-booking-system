namespace BusTicketSystem.Web.DTOs;

public class BookingRequestDto
{
    public int TripId { get; set; }
    public int SeatNumber { get; set; }
    public int CustomerId { get; set; }
}