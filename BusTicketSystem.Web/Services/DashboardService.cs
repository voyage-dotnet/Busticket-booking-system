using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Mapping;
using BusTicketSystem.Web.Repositories;

namespace BusTicketSystem.Web.Services
{
    public class DashboardService: IDashboardService    
    {
        private readonly IDashboardRepository _dashRepo;

        public DashboardService(IDashboardRepository dashRepo)
        {
            _dashRepo = dashRepo;
        }

        public async Task<AgencyOverviewDTO> GetAgencyOverviewAsync(int agencyId)
        {
            var tripIds = await _dashRepo.GetTripIdsByAgencyAsync(agencyId);
            var totalBookings = await _dashRepo.GetBookingCountByTripIdsAsync(tripIds);
            var totalRevenue = await _dashRepo.GetRevenueByTripIdsAsync(tripIds);
            var (avgRating, reviewCount) = await _dashRepo.GetReviewStatsByTripIdsAsync(tripIds);

            return new AgencyOverviewDTO
            {
                TotalTrips = tripIds.Count,
                TotalBookings = totalBookings,
                TotalRevenue = totalRevenue,
                AverageRating = Math.Round(avgRating, 2),
                TotalReviews = reviewCount
            };
        }

        public async Task<List<AgencyTripStatsDTO>> GetAgencyTripStatsAsync(int agencyId)
        {
            var trips = await _dashRepo.GetTripsByAgencyAsync(agencyId);
            var result = new List<AgencyTripStatsDTO>();

            foreach (var trip in trips)
            {
                var bookedSeats = await _dashRepo.GetBookedSeatsByTripAsync(trip.TripId);
                var revenue = await _dashRepo.GetRevenueByTripAsync(trip.TripId);
                result.Add(DashboardMapper.ToAgencyTripStatsDTO(trip, bookedSeats, revenue));
            }

            return result;
        }

        public async Task<List<TopRouteDTO>> GetAgencyTopRoutesAsync(int agencyId)
        {
            return await _dashRepo.GetTopRoutesByAgencyAsync(agencyId);
        }

        public async Task<CustomerOverviewDTO> GetCustomerOverviewAsync(int customerId)
        {
            var totalTrips = await _dashRepo.GetCompletedTripCountByCustomerAsync(customerId);
            var totalSpent = await _dashRepo.GetTotalSpentByCustomerAsync(customerId);
            var totalReviews = await _dashRepo.GetReviewCountByCustomerAsync(customerId);
            var recentBookings = await _dashRepo.GetRecentBookingsByCustomerAsync(customerId);

            return new CustomerOverviewDTO
            {
                TotalTripsTaken = totalTrips,
                TotalAmountSpent = totalSpent,
                TotalReviewsGiven = totalReviews,
                RecentBookings = recentBookings   
            };
        }

        public async Task<PublicStatsDTO> GetPublicStatsAsync()
        {
            return new PublicStatsDTO
            {
                TotalRoutes = await _dashRepo.GetTotalRoutesAsync(),
                TotalCitiesCovered = await _dashRepo.GetTotalCitiesAsync(),
                ActiveAgencies = await _dashRepo.GetTotalAgenciesAsync(),
                TotalTripsAvailable = await _dashRepo.GetTotalTripsAsync()
            };
        }
    }
}
