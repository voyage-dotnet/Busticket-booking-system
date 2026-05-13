using BusTicketSystem.MVC.ViewModels;

using BusTicketSystem.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApiService _apiService;

        public ReviewController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public IActionResult Create(int tripId)
        {
            if (!IsRole("Customer")) return RedirectToLogin();
            return View(new SubmitReviewViewModel { TripId = tripId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SubmitReviewViewModel model)
        {
            if (!IsRole("Customer")) return RedirectToLogin();
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.PostAsync<ReviewResponseViewModel>("api/reviews", model);
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Review submitted successfully!";
                return RedirectToAction("MyReviews");
            }
            
            ModelState.AddModelError(string.Empty, result.Message ?? "Error submitting review.");
            return View(model);
        }

        public async Task<IActionResult> TripReviews(int tripId)
        {
            var result = await _apiService.GetAsync<List<ReviewResponseViewModel>>($"api/reviews/trip/{tripId}", requiresAuth: false);
            ViewBag.TripId = tripId;
            return View(result.Data ?? new List<ReviewResponseViewModel>());
        }

        public async Task<IActionResult> AgencyReviews(int agencyId)
        {
            var result = await _apiService.GetAsync<AgencyReviewSummaryViewModel>($"api/reviews/agency/{agencyId}", requiresAuth: false);
            return View(result.Data ?? new AgencyReviewSummaryViewModel());
        }

        public async Task<IActionResult> MyReviews()
        {
            if (!IsRole("Customer")) return RedirectToLogin();

            var result = await _apiService.GetAsync<List<ReviewResponseViewModel>>("api/reviews/my");
            return View(result.Data ?? new List<ReviewResponseViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsRole("Customer")) return RedirectToLogin();

            var result = await _apiService.GetAsync<List<ReviewResponseViewModel>>("api/reviews/my");
            var review = result.Data?.Find(r => r.ReviewId == id);

            if (review == null)
            {
                TempData["ErrorMessage"] = "Review not found.";
                return RedirectToAction("MyReviews");
            }

            return View(new UpdateReviewViewModel
            {
                ReviewId = review.ReviewId,
                Rating = review.Rating,
                Comment = review.Comment
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateReviewViewModel model)
        {
            if (!IsRole("Customer")) return RedirectToLogin();
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.PutAsync<ReviewResponseViewModel>($"api/reviews/{model.ReviewId}", new { model.Rating, model.Comment });
            if (result.Success)
            {
                TempData["SuccessMessage"] = "Review updated successfully!";
                return RedirectToAction("MyReviews");
            }
            
            ModelState.AddModelError(string.Empty, result.Message ?? "Error updating review.");
            return View(model);
        }

        private bool IsRole(string expectedRole)
        {
            var role = HttpContext.Session.GetString("UserRole");
            var token = HttpContext.Session.GetString("JwtToken");
            return role == expectedRole && !string.IsNullOrEmpty(token);
        }

        private IActionResult RedirectToLogin()
        {
            TempData["ErrorMessage"] = "Please log in as customer to view this page.";
            return RedirectToAction("LoginCustomer", "Auth");
        }
    }
}
