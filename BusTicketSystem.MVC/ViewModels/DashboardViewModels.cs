using System.ComponentModel.DataAnnotations;
namespace BusTicketSystem.MVC.ViewModels
{
    // ── Existing Agency Dashboard VM ────────────────────────────────────────────

    public class AgencyDashboardViewModel
    {
        public int TotalBuses { get; set; }
        public int ActiveTrips { get; set; }
        public int TodayBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalRoutes { get; set; }
        public int TotalDrivers { get; set; }
        public List<RecentTripViewModel> RecentTrips { get; set; } = new();
    }

    public class RecentTripViewModel
    {
        public int TripId { get; set; }
        public string Route { get; set; } = string.Empty;
        public DateTime Departure { get; set; }
        public string Status { get; set; } = string.Empty;
        public int Occupancy { get; set; }
    }

    // ── API Response: GET api/dashboard/agency/overview ─────────────────────────

    public class AgencyOverviewApiViewModel
    {
        public int TotalTrips { get; set; }
        public int TotalBookings { get; set; }
        public int TotalBuses { get; set; }
        public int ActiveTrips { get; set; }
        public int TodayBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalRoutes { get; set; }
        public int TotalDrivers { get; set; }
    }

    // ── API Response: GET api/dashboard/agency/trips ─────────────────────────────

    public class AgencyTripStatViewModel
    {
        public int TripId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Fare { get; set; }
        public int TotalSeats { get; set; }
        public int AvailableSeats { get; set; }
        public string Status { get; set; } = string.Empty;
        public int BookedSeats { get; set; }
        public double OccupancyPercentage { get; set; }
    }

    // ── API Response: GET api/dashboard/customer/overview ───────────────────────

    public class CustomerOverviewApiViewModel
    {
        public int TotalBookings { get; set; }
        public int UpcomingTrips { get; set; }
        public int CompletedTrips { get; set; }
        public decimal TotalSpent { get; set; }
        public int TotalTripsTaken { get; set; }
        public decimal TotalAmountSpent { get; set; }
    }
}
