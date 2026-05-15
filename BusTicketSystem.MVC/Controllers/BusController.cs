using BusTicketSystem.MVC.Services;
using BusTicketSystem.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class BusController : Controller
    {
        private readonly ApiService _apiService;

        public BusController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsAgency() => HttpContext.Session.GetString("UserRole") == "Agency";

        public async Task<IActionResult> Index()
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            var response = await _apiService.GetAsync<List<BusViewModel>>("api/agencies/me/buses");
            return View(response.Data ?? new List<BusViewModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBusViewModel model)
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync<object>("api/buses", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Bus added to fleet successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to add bus.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            // Bug #5 fix: API has no GET api/buses/{id}; fetch all agency buses and find by Id
            var response = await _apiService.GetAsync<List<BusViewModel>>("api/agencies/me/buses");
            var bus = response.Data?.FirstOrDefault(b => b.BusId == id);

            if (bus == null)
            {
                TempData["ErrorMessage"] = "Bus not found.";
                return RedirectToAction(nameof(Index));
            }

            var editModel = new CreateBusViewModel
            {
                RegistrationNumber = bus.RegistrationNumber,
                TotalCapacity      = bus.TotalCapacity,
                BusType            = bus.BusType
            };
            return View(editModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CreateBusViewModel model)
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PutAsync<object>($"api/buses/{id}", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Bus details updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to update bus.");
            return View(model);
        }
    }
}
