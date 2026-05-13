using BusTicketSystem.Web.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusTicketSystem.MVC.ViewModels
{

    public class TripSummaryViewModel
    {
        public int TripId { get; set; }
        public string RouteName { get; set; }
        public string BusNumber { get; set; }
        public DateTime DepartureTime { get; set; }
        public string Status { get; set; }
        public decimal Fare { get; set; }
    }

    public class TripSearchPageViewModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Date { get; set; }
        public IEnumerable<TripSearchResultDTO> Trips { get; set; } = new List<TripSearchResultDTO>();
    }

    public class SeatSelectionViewModel
    {
        public TripDetailDTO Trip { get; set; }
        public SeatLayoutDTO SeatLayout { get; set; }
        public int? SelectedSeat { get; set; }
    }

    public class ReviewJourneyViewModel
    {
        public TripDetailDTO Trip { get; set; }
        public int SeatNumber { get; set; }
        public decimal BaseFare { get; set; }
        public decimal SeatFee { get; set; } = 5.50m;
        public decimal BookingFee { get; set; } = 2.00m;
        public decimal Total => BaseFare + SeatFee + BookingFee;
    }

    public class AgencyDashboardViewModel
    {
        public IEnumerable<MyTripWithOccupancyDTO> Trips { get; set; } = new List<MyTripWithOccupancyDTO>();
        public int TotalTrips => Trips.Count();
        public double AvgOccupancy => Trips.Any() ? Trips.Average(t => t.OccupancyPercentage) : 0;
        public int TotalTicketsSold => Trips.Sum(t => t.BookedSeats);
    }

    public class CreateTripViewModel
    {
        public CreateTripRequestDTO Request { get; set; } = new();
        public IEnumerable<SelectListItem> Routes { get; set; } = new List<SelectListItem>();
    }

    public class EditTripViewModel
    {
        public int TripId { get; set; }
        public UpdateTripRequestDTO Request { get; set; } = new();
    }
}
