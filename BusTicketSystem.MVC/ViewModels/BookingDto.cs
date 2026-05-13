namespace BusTicketSystem.MVC.ViewModels;

public sealed class BookingDto
{
    public int BookingId { get; set; }
    public int TripId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public decimal Fare { get; set; }
    public int SeatNumber { get; set; }
    public string Status { get; set; } = string.Empty;
}
