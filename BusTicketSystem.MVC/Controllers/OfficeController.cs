using BusTicketSystem.MVC.ViewModels;

using BusTicketSystem.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class OfficeController : Controller
    {
        private readonly ApiService _apiService;

        public OfficeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            var response = await _apiService.GetAsync<List<OfficeViewModel>>("api/agencies/me/offices");
            
            var model = new OfficeListViewModel
            {
                Offices = response.Data ?? new List<OfficeViewModel>()
            };

            if (!response.Success) TempData["ErrorMessage"] = response.Message;
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            var response = await _apiService.GetAsync<OfficeViewModel>($"api/offices/{id}");
            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message ?? "Office not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(response.Data);
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();
            return View(new CreateOfficeViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOfficeViewModel model)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();
            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync<OfficeViewModel>("api/agencies/me/offices", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Office created successfully.";
                return RedirectToAction(nameof(Details), new { id = response.Data?.OfficeId });
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Could not create office.");
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            var response = await _apiService.GetAsync<OfficeViewModel>($"api/offices/{id}");
            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message ?? "Office not found.";
                return RedirectToAction(nameof(Index));
            }

            var office = response.Data;
            var model = new UpdateOfficeViewModel
            {
                OfficeMail = office.OfficeMail,
                OfficeContactPersonName = office.OfficeContactPersonName,
                OfficeContactNumber = office.OfficeContactNumber,
                OfficeAddressId = office.OfficeAddressId
            };

            ViewBag.OfficeId = id;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateOfficeViewModel model)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            if (!ModelState.IsValid)
            {
                ViewBag.OfficeId = id;
                return View(model);
            }

            var response = await _apiService.PutAsync<OfficeViewModel>($"api/offices/{id}", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Office updated successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Update failed.");
            ViewBag.OfficeId = id;
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
