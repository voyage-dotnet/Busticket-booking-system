using BusTicketSystem.MVC.ViewModels;
using BusTicketSystem.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApiService _apiService;

        public AdminController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Dashboard()
        {
            if (!IsAdmin()) return RedirectToLogin();

            // Fallback to public stats as backend admin dashboard is missing
            var result = await _apiService.GetAsync<AdminDashboardViewModel>("api/dashboard/public/stats");
            return View(result.Data ?? new AdminDashboardViewModel());
        }

        public async Task<IActionResult> Users()
        {
            if (!IsAdmin()) return RedirectToLogin();

            var result = await _apiService.GetAsync<List<UserDetailViewModel>>("api/admin/users");
            return View(result.Data ?? new List<UserDetailViewModel>());
        }

        public async Task<IActionResult> UserDetail(int id)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var result = await _apiService.GetAsync<UserDetailViewModel>($"api/admin/users/{id}");
            if (!result.Success)
            {
                TempData["ErrorMessage"] = result.Message ?? "User not found.";
                return RedirectToAction(nameof(Users));
            }
            return View(result.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(UpdateUserRoleViewModel model)
        {
            if (!IsAdmin()) return RedirectToLogin();

            var result = await _apiService.PutAsync<object>($"api/admin/users/{model.UserId}/role", new { model.NewRole });
            if (result.Success)
            {
                TempData["SuccessMessage"] = "User role updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = result.Message ?? "Failed to update role.";
            }
            return RedirectToAction(nameof(UserDetail), new { id = model.UserId });
        }

        public async Task<IActionResult> Agencies()
        {
            if (!IsAdmin()) return RedirectToLogin();

            var result = await _apiService.GetAsync<List<AdminAgencyStatsViewModel>>("api/agencies");
            return View(result.Data ?? new List<AdminAgencyStatsViewModel>());
        }

        public async Task<IActionResult> Bookings()
        {
            if (!IsAdmin()) return RedirectToLogin();

            var result = await _apiService.GetAsync<List<AdminBookingViewModel>>("api/admin/bookings");
            return View(result.Data ?? new List<AdminBookingViewModel>());
        }

        public async Task<IActionResult> Trips()
        {
            if (!IsAdmin()) return RedirectToLogin();

            var result = await _apiService.GetAsync<List<AdminTripViewModel>>("api/admin/trips");
            return View(result.Data ?? new List<AdminTripViewModel>());
        }

        private bool IsAdmin()
        {
            var role = HttpContext.Session.GetString("UserRole");
            var token = HttpContext.Session.GetString("JwtToken");
            return role == "Admin" && !string.IsNullOrEmpty(token);
        }

        private IActionResult RedirectToLogin()
        {
            TempData["ErrorMessage"] = "Please log in as Administrator.";
            return RedirectToAction("LoginCustomer", "Auth");
        }
    }
}
