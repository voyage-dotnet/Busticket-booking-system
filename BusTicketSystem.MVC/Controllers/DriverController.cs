using BusTicketSystem.MVC.Models.ViewModels.Agency;
using BusTicketSystem.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    /// <summary>
    /// Handles all driver management UI operations:
    ///   GET  /api/offices/{officeId}/drivers  → list drivers for an office
    ///   POST /api/drivers                     → register a new driver
    ///   PUT  /api/drivers/{id}                → update driver info
    /// All actions require Agency role (JWT in session).
    /// </summary>
    public class DriverController : Controller
    {
        private readonly ApiService _apiService;

        public DriverController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GET /Driver/Index/{officeId}  →  GET /api/offices/{officeId}/drivers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Lists all drivers assigned to a specific office.
        /// </summary>
        public async Task<IActionResult> Index(int officeId)
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            try
            {
                var drivers = await _apiService.GetAsync<List<DriverViewModel>>(
                    $"api/offices/{officeId}/drivers");

                var model = new DriverListViewModel
                {
                    OfficeId = officeId,
                    Drivers = drivers ?? new List<DriverViewModel>()
                };

                // Also fetch office email for breadcrumb display
                var office = await _apiService.GetAsync<OfficeViewModel>($"api/offices/{officeId}");
                ViewBag.OfficeName = office?.OfficeMail ?? $"Office #{officeId}";

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Could not load drivers: {ex.Message}";
                return View(new DriverListViewModel { OfficeId = officeId });
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GET  /Driver/Create  →  show create form
        //  POST /Driver/Create  →  POST /api/drivers
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Shows the form to register a new driver. Pre-populates the office dropdown.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            var model = new CreateDriverViewModel
            {
                AvailableOffices = await GetMyOfficesAsync()
            };
            return View(model);
        }

        /// <summary>
        /// Submits the new driver to POST /api/drivers.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateDriverViewModel model)
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            if (!ModelState.IsValid)
            {
                model.AvailableOffices = await GetMyOfficesAsync();
                return View(model);
            }

            try
            {
                var payload = new
                {
                    model.Name,
                    model.LicenseNumber,
                    model.Phone,
                    model.OfficeId,
                    model.AddressId
                };

                await _apiService.PostAsync<DriverViewModel>("api/drivers", payload);
                TempData["SuccessMessage"] = "Driver registered successfully.";
                return RedirectToAction(nameof(Index), new { officeId = model.OfficeId });
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Could not register driver: {ex.Message}");
                model.AvailableOffices = await GetMyOfficesAsync();
                return View(model);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GET  /Driver/Edit/{id}?officeId={officeId}  →  show form
        //  POST /Driver/Edit/{id}                      →  PUT /api/drivers/{id}
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Shows the update form for an existing driver.
        /// officeId is passed so we can redirect back to the correct office list.
        /// </summary>
        [HttpGet]
        public IActionResult Edit(int id, int officeId)
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            ViewBag.DriverId = id;
            ViewBag.OfficeId = officeId;
            return View(new UpdateDriverViewModel());
        }

        /// <summary>
        /// Submits driver update data to PUT /api/drivers/{id}.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int officeId, UpdateDriverViewModel model)
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            if (!ModelState.IsValid)
            {
                ViewBag.DriverId = id;
                ViewBag.OfficeId = officeId;
                return View(model);
            }

            try
            {
                var payload = new
                {
                    model.Name,
                    model.LicenseNumber,
                    model.Phone,
                    model.OfficeId,
                    model.AddressId
                };

                await _apiService.PutAsync<DriverViewModel>($"api/drivers/{id}", payload);
                TempData["SuccessMessage"] = "Driver updated successfully.";
                return RedirectToAction(nameof(Index), new { officeId });
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Update failed: {ex.Message}");
                ViewBag.DriverId = id;
                ViewBag.OfficeId = officeId;
                return View(model);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Helpers
        // ─────────────────────────────────────────────────────────────────────

        private bool IsAgencyLoggedIn()
        {
            var role = HttpContext.Session.GetString("UserRole");
            var token = HttpContext.Session.GetString("JwtToken");
            return role == "Agency" && !string.IsNullOrEmpty(token);
        }

        private IActionResult RedirectToLogin()
        {
            TempData["ErrorMessage"] = "Please log in as an agency to continue.";
            return RedirectToAction("Login", "Auth");
        }

        private async Task<List<OfficeViewModel>> GetMyOfficesAsync()
        {
            try
            {
                return await _apiService.GetAsync<List<OfficeViewModel>>("api/agencies/me/offices")
                       ?? new List<OfficeViewModel>();
            }
            catch
            {
                return new List<OfficeViewModel>();
            }
        }
    }
}
