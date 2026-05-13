using System;
using System.Collections.Generic;

namespace BusTicketSystem.MVC.ViewModels
{
    public class AgencyOverviewViewModel
    {
        public int TotalTrips { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }

    public class AgencyTripStatsViewModel
    {
        public int TripId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }
        public double OccupancyPercentage { get; set; }
        public decimal Revenue { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class TopRouteViewModel
    {
        public int RouteId { get; set; }
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class CustomerOverviewViewModel
    {
        public int TotalTripsTaken { get; set; }
        public decimal TotalAmountSpent { get; set; }
        public int TotalReviewsGiven { get; set; }
        public List<RecentBookingViewModel> RecentBookings { get; set; } = new();
    }

    public class RecentBookingViewModel
    {
        public int BookingId { get; set; }
        public string Route { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class PublicStatsViewModel
    {
        public int TotalRoutes { get; set; }
        public int TotalCitiesCovered { get; set; }
        public int ActiveAgencies { get; set; }
        public int TotalTripsAvailable { get; set; }
    }

    public class AgencyDashboardCompositeViewModel
    {
        public AgencyOverviewViewModel Overview { get; set; } = new();
        public List<AgencyTripStatsViewModel> TripStats { get; set; } = new();
        public List<TopRouteViewModel> TopRoutes { get; set; } = new();
    }
}
