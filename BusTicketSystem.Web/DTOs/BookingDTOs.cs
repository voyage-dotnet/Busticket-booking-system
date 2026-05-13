namespace BusTicketSystem.Web.DTOs;
public class BookingRequestDTO
{
    public int TripId { get; set; }
    public int SeatNumber { get; set; }
}
public class BookingResponseDTO : BookingRequestDTO
{
    public int BookingId { get; set; }
    public string RouteName { get; set; } = string.Empty;   
    public DateTime DepartureTime { get; set; }
    public decimal Fare { get; set; }
    public string Status { get; set; } = string.Empty;
}
