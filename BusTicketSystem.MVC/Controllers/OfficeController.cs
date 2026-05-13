using BusTicketSystem.MVC.Models.ViewModels.Agency;
using BusTicketSystem.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    /// <summary>
    /// Handles all agency-office UI operations:
    ///   GET  /api/agencies/me/offices      → list own offices
    ///   GET  /api/offices/{id}             → single office detail
    ///   POST /api/agencies/me/offices      → create new office
    ///   PUT  /api/offices/{id}             → update office
    /// All actions require Agency role (JWT in session).
    /// </summary>
    public class OfficeController : Controller
    {
        private readonly ApiService _apiService;

        public OfficeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GET /Office/Index  →  GET /api/agencies/me/offices
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Lists all offices owned by the currently logged-in agency.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            try
            {
                var offices = await _apiService.GetAsync<List<OfficeViewModel>>(
                    "api/agencies/me/offices");

                var model = new OfficeListViewModel
                {
                    Offices = offices ?? new List<OfficeViewModel>()
                };
                return View(model);
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Could not load offices: {ex.Message}";
                return View(new OfficeListViewModel());
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GET /Office/Details/{id}  →  GET /api/offices/{id}
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Shows full detail of a single office.
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            try
            {
                var office = await _apiService.GetAsync<OfficeViewModel>($"api/offices/{id}");
                if (office == null)
                {
                    TempData["ErrorMessage"] = "Office not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(office);
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Could not load office details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GET  /Office/Create  →  show form
        //  POST /Office/Create  →  POST /api/agencies/me/offices
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Shows the form for adding a new office.
        /// </summary>
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            return View(new CreateOfficeViewModel());
        }

        /// <summary>
        /// Submits a new office to POST /api/agencies/me/offices.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateOfficeViewModel model)
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var payload = new
                {
                    model.OfficeMail,
                    model.OfficeContactPersonName,
                    model.OfficeContactNumber,
                    model.OfficeAddressId
                };

                var created = await _apiService.PostAsync<OfficeViewModel>(
                    "api/agencies/me/offices", payload);

                TempData["SuccessMessage"] = "Office created successfully.";
                return RedirectToAction(nameof(Details), new { id = created?.OfficeId });
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Could not create office: {ex.Message}");
                return View(model);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GET  /Office/Edit/{id}  →  load current data & show form
        //  POST /Office/Edit/{id}  →  PUT /api/offices/{id}
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Shows the edit form pre-filled with existing office data.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            try
            {
                var office = await _apiService.GetAsync<OfficeViewModel>($"api/offices/{id}");
                if (office == null)
                {
                    TempData["ErrorMessage"] = "Office not found.";
                    return RedirectToAction(nameof(Index));
                }

                var model = new UpdateOfficeViewModel
                {
                    OfficeMail = office.OfficeMail,
                    OfficeContactPersonName = office.OfficeContactPersonName,
                    OfficeContactNumber = office.OfficeContactNumber,
                    OfficeAddressId = office.OfficeAddressId
                };

                // Pass office id to the view via ViewBag
                ViewBag.OfficeId = id;
                return View(model);
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Error loading form: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Submits updated office data to PUT /api/offices/{id}.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateOfficeViewModel model)
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            if (!ModelState.IsValid)
            {
                ViewBag.OfficeId = id;
                return View(model);
            }

            try
            {
                var payload = new
                {
                    model.OfficeMail,
                    model.OfficeContactPersonName,
                    model.OfficeContactNumber,
                    model.OfficeAddressId
                };

                await _apiService.PutAsync<OfficeViewModel>($"api/offices/{id}", payload);
                TempData["SuccessMessage"] = "Office updated successfully.";
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Update failed: {ex.Message}");
                ViewBag.OfficeId = id;
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
    }
}
