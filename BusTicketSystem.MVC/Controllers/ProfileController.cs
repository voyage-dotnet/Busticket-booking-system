using BusTicketSystem.MVC.Models;
using BusTicketSystem.MVC.Services;
using BusTicketSystem.MVC.Helper;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class ProfileController : Controller
    {
        private readonly VoyageApiClient _apiClient;

        public ProfileController(VoyageApiClient apiClient)
        {
            _apiClient = apiClient;
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

            var result = await _apiClient.GetAsync<CustomerProfileViewModel>($"Profile/Get-customer-profile?Email={Uri.EscapeDataString(email)}");
            
            if (result != null && result.Success)
            {
                return View(result.Data);
            }

            if (result?.StatusCode == 401 || result?.StatusCode == 403)
            {
                TempData["Error"] = "Session expired or unauthorized. Please log in as a customer.";
                return RedirectToAction("LoginCustomer", "Auth");
            }

            var errors = result?.GetErrorList();
            TempData["Error"] = result?.Message ?? (errors != null && errors.Any() ? string.Join(", ", errors) : $"Failed to load profile (Status: {result?.StatusCode}).");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AddAddress()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddAddress(CustomerAddressViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = HttpContext.Session.GetString("JwtToken");
            var email = TokenHelper.GetUserEmail(token);

            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
                return RedirectToAction("LoginCustomer", "Auth");

            var result = await _apiClient.PostAsync<object>($"Profile/Add-customer-address?Email={Uri.EscapeDataString(email)}", model);

            if (result != null && result.Success)
            {
                TempData["Message"] = result.Message;
                return RedirectToAction("Index");
            }

            if (result?.StatusCode == 401 || result?.StatusCode == 403)
            {
                TempData["Error"] = "You are not authorized to perform this action. Please log in as a customer.";
                return RedirectToAction("LoginCustomer", "Auth");
            }

            TempData["Error"] = result?.Message ?? $"Failed to add address (Status: {result?.StatusCode}).";
            var errors = result?.GetErrorList();
            if (errors != null && errors.Any())
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult UpdateEmail()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("LoginCustomer", "Auth");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmail(UpdateEmailViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("LoginCustomer", "Auth");

            // We use model.Email as the 'Current Email' (query param)
            // and model.NewEmail as the 'New Email' (mapped to DTO)
            if (string.IsNullOrEmpty(model.Email))
            {
                ModelState.AddModelError("Email", "Current email is required");
                return View(model);
            }

            var result = await _apiClient.PatchAsync<object>(
                $"Auth/Update-customer-email?Email={Uri.EscapeDataString(model.Email)}", 
                new { Email = model.NewEmail }
            );

            if (result != null && result.Success)
            {
                // Clear session because the old token contains the old email
                HttpContext.Session.Remove("JwtToken");
                HttpContext.Session.Remove("UserEmail");

                TempData["Message"] = "Email updated successfully. Please log in with your new email.";
                return RedirectToAction("LoginCustomer", "Auth");
            }

            if (result?.StatusCode == 401 || result?.StatusCode == 403)
            {
                TempData["Error"] = "You are not authorized to perform this action.";
                return RedirectToAction("LoginCustomer", "Auth");
            }

            TempData["Error"] = result?.Message ?? $"Failed to update email (Status: {result?.StatusCode}).";
            return View(model);
        }
    }
}
