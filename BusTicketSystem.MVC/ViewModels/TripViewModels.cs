using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusTicketSystem.MVC.ViewModels
{
    public class TripSummaryViewModel
    {
        public int TripId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
        public string BusNumber { get; set; } = string.Empty;
        public string BusType { get; set; } = string.Empty;
        public string AgencyName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Fare { get; set; }
    }

    public class TripSearchPageViewModel
    {
        public string From { get; set; } = string.Empty;
        public string To { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public List<TripSummaryViewModel> Trips { get; set; } = new();
    }

    public class TripSeatSelectionViewModel
    {
        public TripDetailDto Trip { get; set; } = new();
        public List<int> OccupiedSeats { get; set; } = new();
        public int? SelectedSeat { get; set; }
    }

    public class ReviewJourneyViewModel
    {
        public TripDetailDto Trip { get; set; } = new();
        public int SeatNumber { get; set; }
        public decimal BaseFare { get; set; }
        public decimal SeatFee { get; set; } = 5.50m;
        public decimal BookingFee { get; set; } = 2.00m;
        public decimal Total => BaseFare + SeatFee + BookingFee;
    }

    public class CreateTripViewModel
    {
        public int RouteId { get; set; }
        public int BusId { get; set; }
        public int DriverId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }
        public IEnumerable<SelectListItem> Routes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Buses { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Drivers { get; set; } = new List<SelectListItem>();
    }

    public class EditTripViewModel
    {
        public int TripId { get; set; }
        public int RouteId { get; set; }
        public int BusId { get; set; }
        public int DriverId { get; set; }
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }
        public string Status { get; set; } = string.Empty;
        public IEnumerable<SelectListItem> Routes { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Buses { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> Drivers { get; set; } = new List<SelectListItem>();
    }

    public sealed class TripDetailDto
    {
        public int TripId { get; set; }
        public int TotalSeats { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
        public string BusNumber { get; set; } = string.Empty;
        public string BusType { get; set; } = string.Empty;
        public string AgencyName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }
    }
}
