using BusTicketSystem.MVC.Services;
using BusTicketSystem.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ApiService _apiService;

        public ProfileController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsLoggedIn() => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));

        public async Task<IActionResult> Index()
        {
            if (!IsLoggedIn()) return RedirectToAction("LoginCustomer", "Auth");

            var email = HttpContext.Session.GetString("UserEmail") ?? string.Empty;
            var response = await _apiService.GetAsync<ProfileViewModel>($"api/Profile/Get-customer-profile?Email={Uri.EscapeDataString(email)}");

            var model = response.Data ?? new ProfileViewModel
            {
                Name = HttpContext.Session.GetString("UserName") ?? email.Split('@')[0],
                Email = email,
                PhoneNumber = string.Empty,
                TotalTrips = 0,
                MemberSince = DateTime.UtcNow
            };

            if (string.IsNullOrWhiteSpace(model.Name))
            {
                model.Name = HttpContext.Session.GetString("UserName") ?? "Customer";
            }
            if (string.IsNullOrWhiteSpace(model.Email))
            {
                model.Email = email;
            }
            if (model.MemberSince == default)
            {
                model.MemberSince = DateTime.UtcNow;
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateProfileViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("LoginCustomer", "Auth");

            if (!ModelState.IsValid) return RedirectToAction(nameof(Index));

            var response = await _apiService.PutAsync<object>("api/profile", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Profile updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Update failed.";
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}
