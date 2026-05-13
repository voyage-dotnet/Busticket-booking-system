using BusTicketSystem.MVC.ViewModels;

using BusTicketSystem.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class AgencyController : Controller
    {
        private readonly ApiService _apiService;

        public AgencyController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _apiService.GetAsync<List<AgencyViewModel>>("api/agencies", requiresAuth: false);

            var model = new AgencyListViewModel
            {
                Agencies = response.Data ?? new List<AgencyViewModel>()
            };
            
            if (!response.Success)
            {
                TempData["ErrorMessage"] = response.Message ?? "Could not load agencies.";
            }
            
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _apiService.GetAsync<AgencyViewModel>($"api/agencies/{id}", requiresAuth: false);

            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message ?? "Agency not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(response.Data);
        }

        public async Task<IActionResult> Profile()
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            var response = await _apiService.GetAsync<AgencyViewModel>("api/agencies/me");
            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message ?? "Could not retrieve your profile.";
                return RedirectToAction("Index", "Home");
            }
            return View(response.Data);
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            var response = await _apiService.GetAsync<AgencyViewModel>("api/agencies/me");
            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message ?? "Could not retrieve your profile.";
                return RedirectToAction(nameof(Profile));
            }

            var agency = response.Data;
            var model = new UpdateAgencyViewModel
            {
                Name = agency.Name,
                ContactPersonName = agency.ContactPersonName,
                Email = agency.Email,
                Phone = agency.Phone
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UpdateAgencyViewModel model)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.PutAsync<AgencyViewModel>("api/agencies/me", model);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Agency profile updated successfully.";
                return RedirectToAction(nameof(Profile));
            }
            
            ModelState.AddModelError(string.Empty, result.Message ?? "Update failed.");
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
            TempData["ErrorMessage"] = "Please log in as an agency to continue.";
            return RedirectToAction("LoginAgency", "Auth");
        }
    }
}
