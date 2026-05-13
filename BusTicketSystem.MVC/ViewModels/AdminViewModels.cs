using System;
using System.Collections.Generic;

namespace BusTicketSystem.MVC.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalAgencies { get; set; }
        public int TotalTrips { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class UserDetailViewModel
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class AdminAgencyStatsViewModel
    {
        public int AgencyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalBuses { get; set; }
        public int TotalTrips { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class AdminBookingViewModel
    {
        public int BookingId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string RouteName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class AdminTripViewModel
    {
        public int TripId { get; set; }
        public string RouteName { get; set; } = string.Empty;
        public string AgencyName { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public int TotalSeats { get; set; }
        public int Occupancy { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class UpdateUserRoleViewModel
    {
        public int UserId { get; set; }
        public string NewRole { get; set; } = string.Empty;
    }
}
