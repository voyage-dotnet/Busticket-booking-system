using BusTicketSystem.MVC.ViewModels;
using BusTicketSystem.MVC.Services;
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

        public async Task<IActionResult> Index(int? officeId)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            List<BusViewModel> buses = new List<BusViewModel>();
            if (officeId.HasValue)
            {
                var response = await _apiService.GetAsync<List<BusViewModel>>($"api/offices/{officeId}/buses");
                buses = response.Data ?? new List<BusViewModel>();
            }
            else
            {
                var offices = await GetMyOfficesAsync();
                foreach (var office in offices)
                {
                    var response = await _apiService.GetAsync<List<BusViewModel>>($"api/offices/{office.OfficeId}/buses");
                    if (response.Success && response.Data != null)
                    {
                        buses.AddRange(response.Data);
                    }
                }
            }
            
            var model = new BusListViewModel
            {
                Buses = buses,
                OfficeId = officeId ?? 0
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? officeId)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            ViewBag.Offices = await GetMyOfficesAsync();
            return View(new CreateBusViewModel { OfficeId = officeId ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBusViewModel model)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            if (!ModelState.IsValid)
            {
                ViewBag.Offices = await GetMyOfficesAsync();
                return View(model);
            }

            var response = await _apiService.PostAsync<BusViewModel>("api/buses", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Bus registered successfully.";
                return RedirectToAction(nameof(Index), new { officeId = model.OfficeId });
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to create bus.");
            ViewBag.Offices = await GetMyOfficesAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int officeId)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            var response = await _apiService.GetAsync<List<BusViewModel>>($"api/offices/{officeId}/buses");
            var bus = response.Data?.FirstOrDefault(b => b.BusId == id);
            
            if (bus == null)
            {
                TempData["ErrorMessage"] = "Bus not found in this office.";
                return RedirectToAction(nameof(Index), new { officeId });
            }


            var model = new UpdateBusViewModel
            {
                RegistrationNumber = bus.RegistrationNumber,
                Type = bus.Type,
                Capacity = bus.Capacity
            };

            ViewBag.BusId = id;
            ViewBag.OfficeId = bus.OfficeId;
            ViewBag.Offices = await GetMyOfficesAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int officeId, UpdateBusViewModel model)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            if (!ModelState.IsValid)
            {
                ViewBag.BusId = id;
                ViewBag.OfficeId = officeId;
                ViewBag.Offices = await GetMyOfficesAsync();
                return View(model);
            }

            var response = await _apiService.PutAsync<BusViewModel>($"api/buses/{id}", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Bus details updated.";
                return RedirectToAction(nameof(Index), new { officeId = officeId });
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Update failed.");
            ViewBag.BusId = id;
            ViewBag.OfficeId = officeId;
            ViewBag.Offices = await GetMyOfficesAsync();
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

        private async Task<List<OfficeViewModel>> GetMyOfficesAsync()
        {
            var response = await _apiService.GetAsync<List<OfficeViewModel>>("api/agencies/me/offices");
            return response.Data ?? new List<OfficeViewModel>();
        }
    }
}
