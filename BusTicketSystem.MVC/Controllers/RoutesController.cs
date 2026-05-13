using BusTicketSystem.MVC.ViewModels;
using BusTicketSystem.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class RoutesController : Controller
    {
        private readonly ApiService _apiService;

        public RoutesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync<List<RouteViewModel>>("api/Routes", requiresAuth: false);
            return View(response.Data ?? new List<RouteViewModel>());
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _apiService.GetAsync<RouteViewModel>($"api/Routes/{id}", requiresAuth: false);
            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message ?? "Route not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(response.Data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();
            return View(new CreateRouteViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateRouteViewModel model)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();
            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync<RouteViewModel>("api/Routes", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Route created successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to create route.");
            return View(model);
        }

        private bool IsAgencyLoggedIn()
        {
            var role = HttpContext.Session.GetString("UserRole");
            var token = HttpContext.Session.GetString("JwtToken");
            return role == "Agency" && !string.IsNullOrEmpty(token);
        }

        private IActionResult RedirectToLogin()
        {
            TempData["ErrorMessage"] = "Please log in as an agency.";
            return RedirectToAction("LoginAgency", "Auth");
        }
    }
}
