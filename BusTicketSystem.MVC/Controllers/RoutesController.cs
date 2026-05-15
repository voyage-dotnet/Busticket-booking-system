using BusTicketSystem.MVC.Services;
using BusTicketSystem.MVC.ViewModels;
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

        private bool IsAgency() => HttpContext.Session.GetString("UserRole") == "Agency";

        public async Task<IActionResult> Index()
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            var response = await _apiService.GetAsync<List<RouteViewModel>>("api/routes");
            return View(response.Data ?? new List<RouteViewModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRouteViewModel model)
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync<object>("api/routes", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "New route added successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to add route.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            var response = await _apiService.GetAsync<RouteViewModel>("api/routes/" + id);
            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = "Route not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new CreateRouteViewModel
            {
                FromCity = response.Data.FromCity,
                ToCity = response.Data.ToCity,
                BreakPoints = response.Data.BreakPoints,
                EstimatedDurationMinutes = response.Data.EstimatedDurationMinutes
            };
            ViewBag.RouteId = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CreateRouteViewModel model)
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            if (!ModelState.IsValid)
            {
                ViewBag.RouteId = id;
                return View(model);
            }

            // API only allows updating BreakPoints and EstimatedDurationMinutes
            var updatePayload = new
            {
                model.BreakPoints,
                model.EstimatedDurationMinutes
            };

            var response = await _apiService.PutAsync<object>($"api/routes/{id}", updatePayload);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Route updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to update route.");
            ViewBag.RouteId = id;
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            var response = await _apiService.DeleteAsync<object>($"api/routes/{id}");
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Route deleted successfully!";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to delete route.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
