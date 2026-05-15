using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;

namespace BusTicketSystem.Web.Mapping
{
    public class DashboardMapper
    {
        public static AgencyTripStatsDTO ToAgencyTripStatsDTO(
        Trip trip, int bookedSeats, decimal revenue)
    {
        var totalSeats = trip.Bus?.Capacity ?? 0;   
 
        return new AgencyTripStatsDTO
        {
            TripId              = trip.TripId,
            RouteName           = $"{trip.Route?.FromCity} → {trip.Route?.ToCity}",
            DepartureTime       = trip.DepartureTime,
            TotalSeats          = totalSeats,
            BookedSeats         = bookedSeats,
            OccupancyPercentage = totalSeats > 0
                                    ? Math.Round((double)bookedSeats / totalSeats * 100, 2)
                                    : 0,
            Revenue             = revenue,
            Status              = trip.DepartureTime > DateTime.UtcNow
                                    ? "Upcoming"
                                    : "Completed"
        };
    }
    }
}
