using BusTicketSystem.MVC.Models.ViewModels.Agency;
using BusTicketSystem.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    /// <summary>
    /// Handles all bus management UI operations:
    ///   GET  /api/offices/{officeId}/buses  → list buses for an office
    ///   POST /api/buses                     → register a new bus
    ///   PUT  /api/buses/{id}                → update bus details
    /// All actions require Agency role (JWT in session).
    /// </summary>
    public class BusController : Controller
    {
        private readonly ApiService _apiService;

        public BusController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GET /Bus/Index/{officeId}  →  GET /api/offices/{officeId}/buses
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Lists all buses registered to a specific office.
        /// </summary>
        public async Task<IActionResult> Index(int officeId)
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            try
            {
                var buses = await _apiService.GetAsync<List<BusViewModel>>(
                    $"api/offices/{officeId}/buses");

                var model = new BusListViewModel
                {
                    OfficeId = officeId,
                    Buses = buses ?? new List<BusViewModel>()
                };

                // Also fetch office name for display
                var office = await _apiService.GetAsync<OfficeViewModel>($"api/offices/{officeId}");
                ViewBag.OfficeName = office?.OfficeMail ?? $"Office #{officeId}";

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Could not load buses: {ex.Message}";
                return View(new BusListViewModel { OfficeId = officeId });
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GET  /Bus/Create  →  show create form
        //  POST /Bus/Create  →  POST /api/buses
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Shows form to register a new bus. Pre-populates the office dropdown.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            var model = new CreateBusViewModel
            {
                AvailableOffices = await GetMyOfficesAsync()
            };
            return View(model);
        }

        /// <summary>
        /// Submits the new bus to POST /api/buses.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBusViewModel model)
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
                    model.OfficeId,
                    model.RegistrationNumber,
                    model.Capacity,
                    model.Type
                };

                await _apiService.PostAsync<BusViewModel>("api/buses", payload);
                TempData["SuccessMessage"] = "Bus registered successfully.";
                return RedirectToAction(nameof(Index), new { officeId = model.OfficeId });
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Could not register bus: {ex.Message}");
                model.AvailableOffices = await GetMyOfficesAsync();
                return View(model);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GET  /Bus/Edit/{id}?officeId={officeId}  →  load form
        //  POST /Bus/Edit/{id}                      →  PUT /api/buses/{id}
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Shows the update form for an existing bus.
        /// officeId is passed so we can redirect back to the correct office list.
        /// </summary>
        [HttpGet]
        public IActionResult Edit(int id, int officeId)
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            // We don't have a GET single-bus endpoint — start with empty form
            // and let the user fill only what they want to change
            ViewBag.BusId = id;
            ViewBag.OfficeId = officeId;
            return View(new UpdateBusViewModel());
        }

        /// <summary>
        /// Submits bus update data to PUT /api/buses/{id}.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int officeId, UpdateBusViewModel model)
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            if (!ModelState.IsValid)
            {
                ViewBag.BusId = id;
                ViewBag.OfficeId = officeId;
                return View(model);
            }

            try
            {
                var payload = new
                {
                    model.RegistrationNumber,
                    model.Capacity,
                    model.Type
                };

                await _apiService.PutAsync<BusViewModel>($"api/buses/{id}", payload);
                TempData["SuccessMessage"] = "Bus updated successfully.";
                return RedirectToAction(nameof(Index), new { officeId });
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Update failed: {ex.Message}");
                ViewBag.BusId = id;
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
