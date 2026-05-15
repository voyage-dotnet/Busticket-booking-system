using BusTicketSystem.MVC.Services;
using BusTicketSystem.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ApiService _apiService;

        public PaymentController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsLoggedIn() => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));
        private bool IsAgency() => HttpContext.Session.GetString("UserRole") == "Agency";

        [HttpGet]
        public async Task<IActionResult> History()
        {
            if (!IsLoggedIn()) return RedirectToAction("LoginCustomer", "Auth");

            var response = await _apiService.GetAsync<List<PaymentHistoryViewModel>>("api/payments/my");
            return View(response.Data ?? new List<PaymentHistoryViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Revenue()
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            var response = await _apiService.GetAsync<AgencyRevenueViewModel>("api/payments/agency/revenue");
            var tripsResponse = await _apiService.GetAsync<List<AgencyTripStatViewModel>>("api/dashboard/agency/trips");
            var topRoutesResponse = await _apiService.GetAsync<List<TopRouteRevenueViewModel>>("api/dashboard/agency/top-routes");

            var model = response.Data ?? new AgencyRevenueViewModel();
            if (model.TotalBookings == 0)
            {
                model.TotalBookings = model.TotalPayments;
            }

            var trips = tripsResponse.Data ?? new List<AgencyTripStatViewModel>();
            if (model.ThisMonthRevenue == 0)
            {
                var now = DateTime.Now;
                model.ThisMonthRevenue = trips
                    .Where(t => t.DepartureTime.Month == now.Month && t.DepartureTime.Year == now.Year)
                    .Sum(t => t.Fare * t.BookedSeats);
            }

            model.RevenueTrend = trips
                .Where(t => t.BookedSeats > 0)
                .GroupBy(t => t.DepartureTime.ToString("MMM"))
                .Select(g => new RevenueTrendPointViewModel
                {
                    Label = g.Key,
                    Amount = g.Sum(t => t.Fare * t.BookedSeats)
                })
                .ToList();

            model.TopRoutes = topRoutesResponse.Data ?? trips
                .Where(t => t.BookedSeats > 0)
                .GroupBy(t => !string.IsNullOrWhiteSpace(t.RouteName) ? t.RouteName : $"{t.FromCity} -> {t.ToCity}")
                .Select(g => new TopRouteRevenueViewModel
                {
                    RouteName = g.Key,
                    BookingCount = g.Sum(t => t.BookedSeats),
                    TotalRevenue = g.Sum(t => t.Fare * t.BookedSeats)
                })
                .OrderByDescending(r => r.TotalRevenue)
                .Take(5)
                .ToList();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Process(PaymentRequestViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("LoginCustomer", "Auth");

            var response = await _apiService.PostAsync<object>("api/payments", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Payment successful!";
                return RedirectToAction("MyBookings", "TicketBooking");
            }

            TempData["ErrorMessage"] = response.Message ?? "Payment failed.";
            return RedirectToAction("Index", "Home");
        }
    }
}
