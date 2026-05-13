using System.ComponentModel.DataAnnotations;

namespace BusTicketSystem.MVC.Models.Booking;

public class BookingListViewModel
{
    public List<BookingItemViewModel> Bookings { get; set; } = new();
}

public class BookingItemViewModel
{
    public int BookingId { get; set; }
    public int TripId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public decimal Fare { get; set; }
    public int SeatNumber { get; set; }
    public string Status { get; set; } = string.Empty;

    public bool IsCancellable => Status == "Booked";

    public string StatusBadgeCss => Status switch
    {
        "Booked"    => "badge-success",
        "Available" => "badge-warning",
        _           => "badge-info"
    };
}

public class SeatSelectionViewModel
{
    public int TripId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public decimal Fare { get; set; }
    public int TotalSeats { get; set; }
    public List<int> AvailableSeats { get; set; } = new();

    [Range(1, 200, ErrorMessage = "Please select a valid seat.")]
    public int SelectedSeat { get; set; }
}

public class BookingConfirmationViewModel
{
    public int BookingId { get; set; }
    public string RouteName { get; set; } = string.Empty;
    public DateTime DepartureTime { get; set; }
    public decimal Fare { get; set; }
    public int SeatNumber { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ConfirmationCode => $"VYG-{BookingId:D4}-XC";
}