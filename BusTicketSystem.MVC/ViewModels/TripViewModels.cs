using System.ComponentModel.DataAnnotations;
namespace BusTicketSystem.MVC.ViewModels
{
    public class TripDetailResponse
    {
        public int TripId { get; set; }
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public string Status { get; set; } = string.Empty;
        public NestedBus Bus { get; set; } = new();
        public NestedAgency Agency { get; set; } = new();
        public NestedAddress BoardingAddress { get; set; } = new();

        public class NestedBus { public string BusType { get; set; } = string.Empty; }
        public class NestedAgency { public string Name { get; set; } = string.Empty; }
        public class NestedAddress { public string Street { get; set; } = string.Empty; public string City { get; set; } = string.Empty; }
    }

    public class TripSearchPageViewModel
    {
        public string? From { get; set; }
        public string? To { get; set; }
        public string? Date { get; set; }
        public List<TripSummaryViewModel> Trips { get; set; } = new();
    }

    public class TripSummaryViewModel
    {
        public int TripId { get; set; }

        // Fields returned by both TripSummaryDTO and TripSearchResultDTO
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }
        public int AvailableSeats { get; set; }
        public string Status { get; set; } = string.Empty;  // Bug #7 fix: was missing

        // Present in TripSummaryDTO (agency view)
        [System.Text.Json.Serialization.JsonPropertyName("totalSeats")]
        public int TotalCapacity { get; set; }

        public string BusType { get; set; } = string.Empty;
        public string AgencyName { get; set; } = string.Empty;
        public string BoardingPoint { get; set; } = "Main Terminal";
    }

    public class TripSeatSelectionViewModel
    {
        public TripSummaryViewModel Trip { get; set; } = new();
        public List<int> OccupiedSeats { get; set; } = new();
    }

    public class ReviewJourneyViewModel
    {
        public TripSummaryViewModel Trip { get; set; } = new();
        public int SeatNumber { get; set; }
        public decimal BaseFare { get; set; }
        public decimal SeatSelectionFee { get; set; } = 5.50m;
        public decimal BookingFee { get; set; } = 2.00m;
        public decimal TotalAmount => BaseFare + SeatSelectionFee + BookingFee;
    }

    // Bug #6 fix: aligned with API's CreateTripRequestDTO
    public class CreateTripViewModel
    {
        [Required, Range(1, int.MaxValue, ErrorMessage = "Route is required")]
        public int RouteId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Bus is required")]
        public int BusId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Driver is required")]
        public int DriverId { get; set; }

        [Required, Range(1, int.MaxValue, ErrorMessage = "Boarding point is required")]
        public int BoardingAddressId { get; set; }

        [Required]
        public DateTime DepartureTime { get; set; }

        [Required]
        public DateTime ArrivalTime { get; set; }

        [Required, Range(1, 100000, ErrorMessage = "Fare must be between 1 and 100,000")]
        public decimal Fare { get; set; }
    }
}
