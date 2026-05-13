using BusTicketSystem.MVC.ViewModels;

using BusTicketSystem.MVC.Services;
using BusTicketSystem.MVC.Helper;
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

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            var email = TokenHelper.GetUserEmail(token);

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                TempData["Error"] = "Session expired or email not found in token. Please log in again.";
                return RedirectToAction("LoginCustomer", "Auth");
            }

            var result = await _apiService.GetAsync<CustomerProfileViewModel>($"api/Profile/Get-customer-profile?Email={Uri.EscapeDataString(email)}");
            
            if (result.Success)
            {
                return View(result.Data);
            }

            if (result.StatusCode == 401 || result.StatusCode == 403)
            {
                TempData["Error"] = "Session expired or unauthorized. Please log in as a customer.";
                return RedirectToAction("LoginCustomer", "Auth");
            }

            TempData["Error"] = result.Message ?? "Failed to load profile.";
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AddAddress() => View();

        [HttpPost]
        public async Task<IActionResult> AddAddress(CustomerAddressViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var token = HttpContext.Session.GetString("JwtToken");
            var email = TokenHelper.GetUserEmail(token);

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return RedirectToAction("LoginCustomer", "Auth");

            var result = await _apiService.PostAsync<object>($"api/Profile/Add-customer-address?Email={Uri.EscapeDataString(email)}", model);

            if (result.Success)
            {
                TempData["Message"] = result.Message;
                return RedirectToAction("Index");
            }

            TempData["Error"] = result.Message ?? "Failed to add address.";
            foreach (var error in result.GetErrorList())
            {
                ModelState.AddModelError("", error);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult UpdateEmail()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
                return RedirectToAction("LoginCustomer", "Auth");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmail(UpdateEmailViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken")))
                return RedirectToAction("LoginCustomer", "Auth");

            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "Current email is required");
                return View(model);
            }

            var result = await _apiService.PatchAsync<object>(
                $"api/Auth/Update-customer-email?Email={Uri.EscapeDataString(model.Email)}", 
                new { Email = model.NewEmail }
            );

            if (result.Success)
            {
                HttpContext.Session.Remove("JwtToken");
                HttpContext.Session.Remove("UserEmail");
                TempData["Message"] = "Email updated successfully. Please log in with your new email.";
                return RedirectToAction("LoginCustomer", "Auth");
            }

            TempData["Error"] = result.Message ?? "Failed to update email.";
            return View(model);
        }
    }
}
