using BusTicketSystem.MVC.Services;
using BusTicketSystem.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class DriverController : Controller
    {
        private readonly ApiService _apiService;

        public DriverController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsAgency() => HttpContext.Session.GetString("UserRole") == "Agency";

        public async Task<IActionResult> Index()
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            var response = await _apiService.GetAsync<List<DriverViewModel>>("api/agencies/me/drivers");
            return View(response.Data ?? new List<DriverViewModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateDriverViewModel model)
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync<object>("api/drivers", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Driver registered successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to register driver.");
            return View(model);
        }
    }
}
