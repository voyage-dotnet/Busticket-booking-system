using BusTicketSystem.MVC.Models.ViewModels.Agency;
using BusTicketSystem.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    /// <summary>
    /// Handles all agency-level UI operations:
    ///   GET  /api/agencies          → public list
    ///   GET  /api/agencies/{id}     → public detail
    ///   GET  /api/agencies/me       → logged-in agency profile
    ///   PUT  /api/agencies/me       → update own profile
    /// </summary>
    public class AgencyController : Controller
    {
        private readonly ApiService _apiService;

        public AgencyController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GET /Agency/Index  →  GET /api/agencies  (Public)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Lists all registered agencies visible to the public.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var agencies = await _apiService.GetAsync<List<AgencyViewModel>>(
                    "api/agencies", requiresAuth: false);

                var model = new AgencyListViewModel
                {
                    Agencies = agencies ?? new List<AgencyViewModel>()
                };
                return View(model);
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Could not load agencies: {ex.Message}";
                return View(new AgencyListViewModel());
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GET /Agency/Details/{id}  →  GET /api/agencies/{id}  (Public)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Shows the public profile of a specific agency.
        /// </summary>
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var agency = await _apiService.GetAsync<AgencyViewModel>(
                    $"api/agencies/{id}", requiresAuth: false);

                if (agency == null)
                {
                    TempData["ErrorMessage"] = "Agency not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(agency);
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Could not load agency: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GET /Agency/Profile  →  GET /api/agencies/me  (Agency role)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Shows the full profile of the currently logged-in agency.
        /// </summary>
        public async Task<IActionResult> Profile()
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            try
            {
                var agency = await _apiService.GetAsync<AgencyViewModel>("api/agencies/me");
                if (agency == null)
                {
                    TempData["ErrorMessage"] = "Could not retrieve your profile.";
                    return RedirectToAction("Index", "Home");
                }
                return View(agency);
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Error fetching profile: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  GET  /Agency/EditProfile  →  load form for PUT /api/agencies/me
        //  POST /Agency/EditProfile  →  PUT /api/agencies/me  (Agency role)
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>
        /// Shows the edit form pre-populated with the current agency data.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            try
            {
                var agency = await _apiService.GetAsync<AgencyViewModel>("api/agencies/me");
                if (agency == null)
                {
                    TempData["ErrorMessage"] = "Could not retrieve your profile.";
                    return RedirectToAction(nameof(Profile));
                }

                // Pre-populate the form from the current profile
                var model = new UpdateAgencyViewModel
                {
                    Name = agency.Name,
                    ContactPersonName = agency.ContactPersonName,
                    Email = agency.Email,
                    Phone = agency.Phone
                };
                return View(model);
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = $"Error loading form: {ex.Message}";
                return RedirectToAction(nameof(Profile));
            }
        }

        /// <summary>
        /// Submits updated agency info to PUT /api/agencies/me.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UpdateAgencyViewModel model)
        {
            if (!IsAgencyLoggedIn())
                return RedirectToLogin();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var payload = new
                {
                    model.Name,
                    model.ContactPersonName,
                    model.Email,
                    model.Phone
                };

                await _apiService.PutAsync<AgencyViewModel>("api/agencies/me", payload);
                TempData["SuccessMessage"] = "Agency profile updated successfully.";
                return RedirectToAction(nameof(Profile));
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(string.Empty, $"Update failed: {ex.Message}");
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
