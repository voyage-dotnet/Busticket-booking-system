using BusTicketSystem.MVC.ViewModels;
using BusTicketSystem.MVC.Services;
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

        public async Task<IActionResult> Index(int? officeId)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            List<DriverViewModel> drivers = new List<DriverViewModel>();
            if (officeId.HasValue)
            {
                var response = await _apiService.GetAsync<List<DriverViewModel>>($"api/offices/{officeId}/drivers");
                drivers = response.Data ?? new List<DriverViewModel>();
            }
            else
            {
                var offices = await GetMyOfficesAsync();
                foreach (var office in offices)
                {
                    var response = await _apiService.GetAsync<List<DriverViewModel>>($"api/offices/{office.OfficeId}/drivers");
                    if (response.Success && response.Data != null)
                    {
                        drivers.AddRange(response.Data);
                    }
                }
            }
            
            var model = new DriverListViewModel
            {
                Drivers = drivers,
                OfficeId = officeId ?? 0
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create(int? officeId)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            ViewBag.Offices = await GetMyOfficesAsync();
            return View(new CreateDriverViewModel { OfficeId = officeId ?? 0 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDriverViewModel model)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            if (!ModelState.IsValid)
            {
                ViewBag.Offices = await GetMyOfficesAsync();
                return View(model);
            }

            var response = await _apiService.PostAsync<DriverViewModel>("api/drivers", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Driver registered successfully.";
                return RedirectToAction(nameof(Index), new { officeId = model.OfficeId });
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to create driver.");
            ViewBag.Offices = await GetMyOfficesAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id, int officeId)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            var response = await _apiService.GetAsync<List<DriverViewModel>>($"api/offices/{officeId}/drivers");
            var driver = response.Data?.FirstOrDefault(d => d.DriverId == id);
            
            if (driver == null)
            {
                TempData["ErrorMessage"] = "Driver not found in this office.";
                return RedirectToAction(nameof(Index), new { officeId });
            }


            var model = new UpdateDriverViewModel
            {
                Name = driver.Name,
                LicenseNumber = driver.LicenseNumber,
                Phone = driver.Phone,
                OfficeId = driver.OfficeId,
                AddressId = driver.AddressId
            };

            ViewBag.DriverId = id;
            ViewBag.OfficeId = driver.OfficeId;
            ViewBag.Offices = await GetMyOfficesAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int officeId, UpdateDriverViewModel model)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            if (!ModelState.IsValid)
            {
                ViewBag.DriverId = id;
                ViewBag.OfficeId = officeId;
                ViewBag.Offices = await GetMyOfficesAsync();
                return View(model);
            }

            var response = await _apiService.PutAsync<DriverViewModel>($"api/drivers/{id}", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Driver details updated.";
                return RedirectToAction(nameof(Index), new { officeId = officeId });
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Update failed.");
            ViewBag.DriverId = id;
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
