using BusTicketSystem.MVC.Services;
using BusTicketSystem.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly ApiService _apiService;

        public DashboardController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsAgency() => HttpContext.Session.GetString("UserRole") == "Agency";

        public async Task<IActionResult> Agency()
        {
            if (!IsAgency())
            {
                TempData["ErrorMessage"] = "Unauthorized access.";
                return RedirectToAction("LoginAgency", "Auth");
            }

            var overviewResponse = await _apiService.GetAsync<AgencyOverviewApiViewModel>("api/dashboard/agency/overview");
            var tripsResponse = await _apiService.GetAsync<List<AgencyTripStatViewModel>>("api/dashboard/agency/trips");
            var busesResponse = await _apiService.GetAsync<List<BusViewModel>>("api/agencies/me/buses");
            var driversResponse = await _apiService.GetAsync<List<DriverViewModel>>("api/agencies/me/drivers");
            var routesResponse = await _apiService.GetAsync<List<RouteViewModel>>("api/routes");
            var revenueResponse = await _apiService.GetAsync<AgencyRevenueViewModel>("api/payments/agency/revenue");

            var overview = overviewResponse.Data;
            var trips = tripsResponse.Data ?? new List<AgencyTripStatViewModel>();
            var model = new AgencyDashboardViewModel
            {
                TotalBuses = busesResponse.Data?.Count ?? overview?.TotalBuses ?? 0,
                ActiveTrips = trips.Count(t => t.Status != "Cancelled" && t.Status != "Completed" && t.DepartureTime >= DateTime.Now),
                TodayBookings = overview?.TotalBookings ?? trips.Sum(t => t.BookedSeats),
                TotalRevenue = revenueResponse.Data?.TotalRevenue ?? overview?.TotalRevenue ?? 0m,
                TotalRoutes = routesResponse.Data?.Count ?? overview?.TotalRoutes ?? 0,
                TotalDrivers = driversResponse.Data?.Count ?? overview?.TotalDrivers ?? 0,
                RecentTrips = trips
                    .Take(5)
                    .Select(t => new RecentTripViewModel
                    {
                        TripId = t.TripId,
                        Route = !string.IsNullOrWhiteSpace(t.RouteName) ? t.RouteName : $"{t.FromCity} -> {t.ToCity}",
                        Departure = t.DepartureTime,
                        Status = t.Status,
                        Occupancy = (int)t.OccupancyPercentage
                    })
                    .ToList()
            };

            return View(model);
        }

        public async Task<IActionResult> Customer()
        {
            if (HttpContext.Session.GetString("UserRole") != "Customer")
            {
                return RedirectToAction("LoginCustomer", "Auth");
            }

            var overviewResponse = await _apiService.GetAsync<CustomerOverviewApiViewModel>("api/dashboard/customer/overview");
            var bookingsResponse = await _apiService.GetAsync<List<BookingViewModel>>("api/bookings/my");
            var paymentsResponse = await _apiService.GetAsync<List<PaymentHistoryViewModel>>("api/payments/my");

            var bookings = bookingsResponse.Data ?? new List<BookingViewModel>();
            var payments = paymentsResponse.Data ?? new List<PaymentHistoryViewModel>();
            var now = DateTime.Now;

            var model = new CustomerOverviewApiViewModel
            {
                TotalBookings = bookings.Count,
                UpcomingTrips = bookings.Count(b => b.Status != "Cancelled" && b.DepartureTime >= now),
                CompletedTrips = bookings.Count(b => b.Status == "Completed" || (b.Status != "Cancelled" && b.DepartureTime < now)),
                TotalSpent = payments
                    .Where(p => (p.PaymentStatus == "Success" || p.Status == "Success") && p.BookingStatus != "Cancelled")
                    .Sum(p => p.Amount)
            };

            if (model.TotalBookings == 0 && overviewResponse.Data != null)
            {
                model.TotalBookings = overviewResponse.Data.TotalBookings;
                model.UpcomingTrips = overviewResponse.Data.UpcomingTrips;
                model.CompletedTrips = overviewResponse.Data.CompletedTrips != 0
                    ? overviewResponse.Data.CompletedTrips
                    : overviewResponse.Data.TotalTripsTaken;
                model.TotalSpent = overviewResponse.Data.TotalSpent != 0
                    ? overviewResponse.Data.TotalSpent
                    : overviewResponse.Data.TotalAmountSpent;
            }

            return View(model);
        }
    }
}
