using BusTicketSystem.MVC.Services;
using BusTicketSystem.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class OfficesController : Controller
    {
        private readonly ApiService _apiService;

        public OfficesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsAgency() => HttpContext.Session.GetString("UserRole") == "Agency";

        public async Task<IActionResult> Index()
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            var response = await _apiService.GetAsync<List<OfficeViewModel>>("api/agencies/me/offices");
            return View(response.Data ?? new List<OfficeViewModel>());
        }

        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateOfficeViewModel model)
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync<object>("api/agencies/me/offices", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Branch office added successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to add office.");
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            var response = await _apiService.GetAsync<OfficeViewModel>($"api/offices/{id}");
            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = "Office not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new CreateOfficeViewModel
            {
                OfficeContactPersonName = response.Data.OfficeContactPersonName,
                OfficeMail = response.Data.OfficeMail,
                OfficeContactNumber = response.Data.OfficeContactNumber,
                Address = response.Data.Address
            };
            ViewBag.OfficeId = id;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CreateOfficeViewModel model)
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            if (!ModelState.IsValid)
            {
                ViewBag.OfficeId = id;
                return View(model);
            }

            var response = await _apiService.PutAsync<object>($"api/offices/{id}", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Office updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to update office.");
            ViewBag.OfficeId = id;
            return View(model);
        }
        [HttpGet]
        public async Task<IActionResult> AllAgencies()
        {
            var response = await _apiService.GetAsync<List<PublicAgencyViewModel>>("api/agencies");
            return View(response.Data ?? new List<PublicAgencyViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> AgencyProfile(int id)
        {
            var response = await _apiService.GetAsync<PublicAgencyViewModel>($"api/agencies/{id}");
            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = "Agency not found.";
                return RedirectToAction(nameof(AllAgencies));
            }

            var model = response.Data;
            
            // Try to fetch real rating from the review service
            var reviewResponse = await _apiService.GetAsync<AgencyReviewSummaryViewModel>($"api/reviews/agency/{id}");
            if (reviewResponse.Success && reviewResponse.Data != null)
            {
                model.Rating = reviewResponse.Data.AverageRating;
                // We could also add TotalReviews to PublicAgencyViewModel if we wanted
            }

            return View(model);
        }
    }
}
