namespace BusTicketSystem.MVC.Models.Trip;

public class TripSearchViewModel
{
    public string From { get; set; } = "";
    public string To { get; set; } = "";
    public DateTime Date { get; set; }
    public List<TripSearchResultDto> Results { get; set; } = new();
}

public class TripSearchResultDto
{
    public int TripId { get; set; }
    public string FromCity { get; set; } = "";
    public string ToCity { get; set; } = "";
    public DateTime DepartureTime { get; set; }
    public DateTime ArrivalTime { get; set; }
    public decimal Fare { get; set; }
    public int AvailableSeats { get; set; }
    
    public string BusType { get; set; } = "";
    public string AgencyName { get; set; } = "";
    public string Status { get; set; } = "";
}