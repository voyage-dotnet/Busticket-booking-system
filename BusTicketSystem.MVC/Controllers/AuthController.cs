using BusTicketSystem.MVC.Services;
using BusTicketSystem.MVC.ViewModels;
using BusTicketSystem.MVC.Helper;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;

        public AuthController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult LoginCustomer() => View();

        [HttpPost]
        public async Task<IActionResult> LoginCustomer(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync<LoginResponseViewModel>("api/Auth/login-customer", model);

            if (response.Success && response.Data != null)
            {
                HttpContext.Session.SetString("JwtToken", response.Data.Token);
                HttpContext.Session.SetString("UserEmail", model.Email);
                HttpContext.Session.SetString("UserRole", TokenHelper.GetUserRole(response.Data.Token) ?? "Customer");
                HttpContext.Session.SetString("UserName", TokenHelper.GetUserName(response.Data.Token) ?? model.Email.Split('@')[0]);
                
                TempData["SuccessMessage"] = "Logged in successfully!";
                return RedirectToAction("Customer", "Dashboard");
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterCustomer() => View();

        [HttpPost]
        public async Task<IActionResult> RegisterCustomer(RegisterCustomerViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync<object>("api/Auth/Register-customer", model);

            if (response.Success)
            {
                TempData["SuccessMessage"] = "Account created! Please log in.";
                return RedirectToAction(nameof(LoginCustomer));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Registration failed.");
            return View(model);
        }

        [HttpGet]
        public IActionResult LoginAgency() => View();

        [HttpPost]
        public async Task<IActionResult> LoginAgency(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync<LoginResponseViewModel>("api/Auth/Login-agency", model);

            if (response.Success && response.Data != null)
            {
                HttpContext.Session.SetString("JwtToken", response.Data.Token);
                HttpContext.Session.SetString("UserEmail", model.Email);
                HttpContext.Session.SetString("UserRole", TokenHelper.GetUserRole(response.Data.Token) ?? "Agency");
                HttpContext.Session.SetString("UserName", TokenHelper.GetUserName(response.Data.Token) ?? model.Email.Split('@')[0]);
                
                TempData["SuccessMessage"] = "Agency logged in successfully!";
                return RedirectToAction("Agency", "Dashboard");
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Invalid agency login.");
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterAgency() => View();

        [HttpPost]
        public async Task<IActionResult> RegisterAgency(RegisterAgencyViewModel model)
        {

            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync<object>("api/Auth/Register-agency", model);

            if (response.Success)
            {
                TempData["SuccessMessage"] = "Agency registered successfully. Please login.";
                return RedirectToAction(nameof(LoginAgency));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to register agency.");
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Logged out successfully.";
            return RedirectToAction(nameof(LoginCustomer));
        }
    }
}
