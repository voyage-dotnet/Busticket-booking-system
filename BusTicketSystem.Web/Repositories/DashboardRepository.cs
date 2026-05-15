using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BusTicketSystem.Web.Repositories
{
    public class DashboardRepository: IDashboardRepository
    {
        private readonly BusTicketDbContext _db;

        public DashboardRepository(BusTicketDbContext db)
        {
            _db = db;
        }

        public async Task<List<int>> GetTripIdsByAgencyAsync(int agencyId)
        {
            return await _db.Trips
                .Where(t => t.Bus.Office.AgencyId == agencyId)
                .Select(t => t.TripId)
                .ToListAsync();
        }

        public async Task<int> GetBookingCountByTripIdsAsync(List<int> tripIds)
        {
            return await _db.Bookings
                .CountAsync(b => b.TripId.HasValue
                              && tripIds.Contains(b.TripId.Value)
                              && b.Status != "Cancelled");
        }

        public async Task<decimal> GetRevenueByTripIdsAsync(List<int> tripIds)
        {
            return await _db.Payments
                .Where(p => p.Booking.TripId.HasValue
                         && tripIds.Contains(p.Booking.TripId.Value)
                         && p.PaymentStatus == "Success")
                .SumAsync(p => p.Amount ?? 0m);
        }

        public async Task<(double AvgRating, int Count)> GetReviewStatsByTripIdsAsync(List<int> tripIds)
        {
            var stats = await _db.Reviews
                .Where(r => tripIds.Contains(r.TripId))
                .GroupBy(r => 1)
                .Select(g => new
                {
                    Avg = g.Average(r => (double)r.Rating),
                    Count = g.Count()
                })
                .FirstOrDefaultAsync();

            return stats == null ? (0, 0) : (stats.Avg, stats.Count);
        }

        public async Task<List<Trip>> GetTripsByAgencyAsync(int agencyId)
        {
            return await _db.Trips
                .Where(t => t.Bus.Office.AgencyId == agencyId)
                .Include(t => t.Route)
                .Include(t => t.Bus)
                .OrderByDescending(t => t.DepartureTime)
                .ToListAsync();
        }

        public async Task<int> GetBookedSeatsByTripAsync(int tripId)
        {
            return await _db.Bookings
                .CountAsync(b => b.TripId == tripId && b.Status != "Cancelled");
        }

        public async Task<decimal> GetRevenueByTripAsync(int tripId)
        {
            return await _db.Payments
                .Where(p => p.Booking.TripId == tripId
                         && p.PaymentStatus == "Success")
                .SumAsync(p => p.Amount ?? 0m);
        }
        public async Task<List<TopRouteDTO>> GetTopRoutesByAgencyAsync(int agencyId)
        {
            return await _db.Payments
                .Where(p => p.Booking.Trip!.Bus.Office.AgencyId == agencyId
                         && p.PaymentStatus == "Success"
                         && p.Booking.TripId.HasValue)
                .GroupBy(p => new
                {
                    p.Booking.Trip!.RouteId,
                    p.Booking.Trip.Route.FromCity,
                    p.Booking.Trip.Route.ToCity
                })
                .Select(g => new TopRouteDTO
                {
                    RouteId = g.Key.RouteId,
                    FromCity = g.Key.FromCity,
                    ToCity = g.Key.ToCity,
                    BookingCount = g.Count(),
                    TotalRevenue = g.Sum(p => p.Amount ?? 0m)
                })
                .OrderByDescending(r => r.BookingCount)
                .Take(10)
                .ToListAsync();
        }

        public async Task<int> GetCompletedTripCountByCustomerAsync(int customerId)
        {
            return await _db.Payments
                .CountAsync(p => p.CustomerId == customerId
                              && p.PaymentStatus == "Success"
                              && p.Booking.Status != "Cancelled");
        }

        public async Task<decimal> GetTotalSpentByCustomerAsync(int customerId)
        {
            return await _db.Payments
                .Where(p => p.CustomerId == customerId
                         && p.PaymentStatus == "Success")
                .SumAsync(p => p.Amount ?? 0m);
        }

        public async Task<int> GetReviewCountByCustomerAsync(int customerId)
        {
            return await _db.Reviews
                .CountAsync(r => r.CustomerId == customerId);
        }
        public async Task<List<RecentBookingDTO>> GetRecentBookingsByCustomerAsync(int customerId)
        {
            return await _db.Payments
                .Where(p => p.CustomerId == customerId
                         && p.Booking.TripId.HasValue)
                .OrderByDescending(p => p.Booking.Trip!.DepartureTime)
                .Take(5)
                .Select(p => new RecentBookingDTO
                {
                    BookingId = p.BookingId,
                    Route = p.Booking.Trip!.Route.FromCity + " → " + p.Booking.Trip.Route.ToCity,
                    Date = p.Booking.Trip.DepartureTime,
                    Status = p.Booking.Status
                })
                .ToListAsync();
        }

        public async Task<int> GetTotalRoutesAsync()
            => await _db.Routes.CountAsync();

        public async Task<int> GetTotalCitiesAsync()
        {
            var fromCities = await _db.Routes.Select(r => r.FromCity).ToListAsync();
            var toCities = await _db.Routes.Select(r => r.ToCity).ToListAsync();
            return fromCities.Union(toCities).Distinct().Count();
        }

        public async Task<int> GetTotalAgenciesAsync()
            => await _db.Agencies.CountAsync();

        public async Task<int> GetTotalTripsAsync()
            => await _db.Trips.CountAsync();
    }
}
