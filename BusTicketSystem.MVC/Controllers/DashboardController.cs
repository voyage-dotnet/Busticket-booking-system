using BusTicketSystem.MVC.ViewModels;

using BusTicketSystem.MVC.Services;
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

        public async Task<IActionResult> Agency()
        {
            if (!IsRole("Agency")) return RedirectToLogin();

            var model = new AgencyDashboardCompositeViewModel();
            
            var overviewRes = await _apiService.GetAsync<AgencyOverviewViewModel>("api/dashboard/agency/overview");
            if (overviewRes.Success && overviewRes.Data != null) model.Overview = overviewRes.Data;

            var tripsRes = await _apiService.GetAsync<List<AgencyTripStatsViewModel>>("api/dashboard/agency/trips");
            if (tripsRes.Success && tripsRes.Data != null) model.TripStats = tripsRes.Data;

            var routesRes = await _apiService.GetAsync<List<TopRouteViewModel>>("api/dashboard/agency/top-routes");
            if (routesRes.Success && routesRes.Data != null) model.TopRoutes = routesRes.Data;

            return View(model);
        }

        public async Task<IActionResult> Customer()
        {
            if (!IsRole("Customer")) return RedirectToLogin();

            var result = await _apiService.GetAsync<CustomerOverviewViewModel>("api/dashboard/customer/overview");
            return View(result.Data ?? new CustomerOverviewViewModel());
        }

        public async Task<IActionResult> PublicStats()
        {
            var result = await _apiService.GetAsync<PublicStatsViewModel>("api/dashboard/public/stats", requiresAuth: false);
            return View(result.Data ?? new PublicStatsViewModel());
        }

        private bool IsRole(string expectedRole)
        {
            var role = HttpContext.Session.GetString("UserRole");
            var token = HttpContext.Session.GetString("JwtToken");
            return role == expectedRole && !string.IsNullOrEmpty(token);
        }

        private IActionResult RedirectToLogin()
        {
            TempData["ErrorMessage"] = "Please log in to view this page.";
            return RedirectToAction("LoginCustomer", "Auth");
        }
    }
}
