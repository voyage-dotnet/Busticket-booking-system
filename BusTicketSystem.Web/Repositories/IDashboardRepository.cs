using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Repositories
{
    public interface IDashboardRepository
    {
        // ── Agency ──────────────────────────────────────────────────────────────────
        Task<List<int>> GetTripIdsByAgencyAsync(int agencyId);
        Task<int> GetBookingCountByTripIdsAsync(List<int> tripIds);
        Task<decimal> GetRevenueByTripIdsAsync(List<int> tripIds);
        Task<(double AvgRating, int Count)> GetReviewStatsByTripIdsAsync(List<int> tripIds);
        Task<List<Trip>> GetTripsByAgencyAsync(int agencyId);
        Task<int> GetBookedSeatsByTripAsync(int tripId);
        Task<decimal> GetRevenueByTripAsync(int tripId);
        Task<List<TopRouteDTO>> GetTopRoutesByAgencyAsync(int agencyId);

        // ── Customer ────────────────────────────────────────────────────────────────
        Task<int> GetCompletedTripCountByCustomerAsync(int customerId);
        Task<decimal> GetTotalSpentByCustomerAsync(int customerId);
        Task<int> GetReviewCountByCustomerAsync(int customerId);
        Task<List<RecentBookingDTO>> GetRecentBookingsByCustomerAsync(int customerId);

        // ── Public ──────────────────────────────────────────────────────────────────
        Task<int> GetTotalRoutesAsync();
        Task<int> GetTotalCitiesAsync();
        Task<int> GetTotalAgenciesAsync();
        Task<int> GetTotalTripsAsync();
    }
}
