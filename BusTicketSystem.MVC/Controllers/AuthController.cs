
using BusTicketSystem.MVC.DTO;
using BusTicketSystem.MVC.Models;
using BusTicketSystem.MVC.Services;
using BusTicketSystem.MVC.Helper;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly VoyageApiClient _apiClient;

        public AuthController(VoyageApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        [HttpGet]
        public IActionResult RegisterCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterCustomer(RegisterCustomer model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _apiClient.PostAsync<object>("Auth/Register-customer", model);

            if (result != null && result.Success)
            {
                TempData["Message"] = result.Message;
                return RedirectToAction("LoginCustomer");
            }
            else
            {
                TempData["Error"] = result?.Message ?? "An error occurred during registration.";
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
        }

        [HttpGet]
        public IActionResult LoginCustomer()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginCustomer(Login model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _apiClient.PostAsync<LoginResponseDTO>("Auth/login-customer", model);

            if (result != null && result.Success && result.Data != null)
            {
                HttpContext.Session.SetString("JwtToken", result.Data.Token);
                HttpContext.Session.SetString("UserEmail", model.Email); 
                TempData["Message"] = result.Message;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Error"] = result?.Message ?? "Invalid login attempt.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult RegisterAgency()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAgency(RegisterAgency model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _apiClient.PostAsync<object>("Auth/Register-agency", model);

            if (result != null && result.Success)
            {
                TempData["Message"] = result.Message;
                return RedirectToAction("LoginAgency");
            }
            else
            {
                TempData["Error"] = result?.Message ?? "An error occurred during registration.";
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
        }

        [HttpGet]
        public IActionResult LoginAgency()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAgency(Login model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _apiClient.PostAsync<LoginResponseDTO>("Auth/Login-agency", model);

            if (result != null && result.Success && result.Data != null)
            {
                HttpContext.Session.SetString("JwtToken", result.Data.Token);
                TempData["Message"] = result.Message;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Error"] = result?.Message ?? "Invalid login attempt.";
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult ForgotPasswordCustomer() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordCustomer(UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            
            var result = await _apiClient.PatchAsync<object>($"Auth/Customer-forget-password?Email={Uri.EscapeDataString(model.Email)}", new { Password = model.Password });
            if (result != null && result.Success)
            {
                TempData["Message"] = "Password reset link sent or password updated successfully.";
                return RedirectToAction("LoginCustomer");
            }
            TempData["Error"] = result?.Message ?? "Failed to reset password.";
            return View(model);
        }

        [HttpGet]
        public IActionResult ForgotPasswordAgency() => View();

        [HttpPost]
        public async Task<IActionResult> ForgotPasswordAgency(UpdatePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _apiClient.PatchAsync<object>($"Auth/Agency-forget-password?Email={Uri.EscapeDataString(model.Email)}", new { Password = model.Password });
            if (result != null && result.Success)
            {
                TempData["Message"] = "Password reset link sent or password updated successfully.";
                return RedirectToAction("LoginAgency");
            }
            TempData["Error"] = result?.Message ?? "Failed to reset password.";
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