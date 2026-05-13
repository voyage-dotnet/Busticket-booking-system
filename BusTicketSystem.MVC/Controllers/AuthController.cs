using BusTicketSystem.MVC.ViewModels;

using BusTicketSystem.MVC.Services;
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
        public IActionResult RegisterCustomer() => View();

        [HttpPost]
        public async Task<IActionResult> RegisterCustomer(RegisterCustomer model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.PostAsync<object>("api/Auth/Register-customer", model, requiresAuth: false);

            if (result.Success)
            {
                TempData["Message"] = result.Message;
                return RedirectToAction("LoginCustomer");
            }

            TempData["Error"] = result.Message ?? "An error occurred during registration.";
            foreach (var error in result.GetErrorList())
            {
                ModelState.AddModelError("", error);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult LoginCustomer() => View();

        [HttpPost]
        public async Task<IActionResult> LoginCustomer(Login model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.PostAsync<LoginResponseViewModel>("api/Auth/login-customer", model, requiresAuth: false);

            if (result.Success && result.Data != null)
            {
                HttpContext.Session.SetString("JwtToken", result.Data.Token);
                HttpContext.Session.SetString("UserEmail", model.Email ?? string.Empty); 
                TempData["Message"] = result.Message;
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = result.Message ?? "Invalid login attempt.";
            return View(model);
        }

        [HttpGet]
        public IActionResult RegisterAgency() => View();

        [HttpPost]
        public async Task<IActionResult> RegisterAgency(RegisterAgency model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.PostAsync<object>("api/Auth/Register-agency", model, requiresAuth: false);

            if (result.Success)
            {
                TempData["Message"] = result.Message;
                return RedirectToAction("LoginAgency");
            }

            TempData["Error"] = result.Message ?? "An error occurred during registration.";
            foreach (var error in result.GetErrorList())
            {
                ModelState.AddModelError("", error);
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult LoginAgency() => View();

        [HttpPost]
        public async Task<IActionResult> LoginAgency(Login model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.PostAsync<LoginResponseViewModel>("api/Auth/Login-agency", model, requiresAuth: false);

            if (result.Success && result.Data != null)
            {
                HttpContext.Session.SetString("JwtToken", result.Data.Token);
                TempData["Message"] = result.Message;
                return RedirectToAction("Index", "Home");
            }

            TempData["Error"] = result.Message ?? "Invalid login attempt.";
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordCustomer() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordCustomer(UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            
            var result = await _apiService.PatchAsync<object>($"api/Auth/Customer-forget-password?Email={Uri.EscapeDataString(model.Email)}", new { Password = model.Password }, requiresAuth: false);
            if (result.Success)
            {
                TempData["Message"] = "Password reset link sent or password updated successfully.";
                return RedirectToAction("LoginCustomer");
            }
            TempData["Error"] = result.Message ?? "Failed to reset password.";
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordAgency() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordAgency(UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.PatchAsync<object>($"api/Auth/Agency-forget-password?Email={Uri.EscapeDataString(model.Email)}", new { Password = model.Password }, requiresAuth: false);
            if (result.Success)
            {
                TempData["Message"] = "Password reset link sent or password updated successfully.";
                return RedirectToAction("LoginAgency");
            }
            TempData["Error"] = result.Message ?? "Failed to reset password.";
            return View(model);
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("JwtToken");
            HttpContext.Session.Remove("UserEmail");
            TempData["Message"] = "Logged out successfully";
            return RedirectToAction("Index", "Home");
        }
    }
}