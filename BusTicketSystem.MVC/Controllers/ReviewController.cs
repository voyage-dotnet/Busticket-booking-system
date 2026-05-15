using BusTicketSystem.MVC.Services;
using BusTicketSystem.MVC.ViewModels;
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

        private bool IsCustomer() => HttpContext.Session.GetString("UserRole") == "Customer";

        [HttpGet]
        public async Task<IActionResult> Submit(int bookingId)
        {
            if (!IsCustomer()) return RedirectToAction("LoginCustomer", "Auth");

            var response = await _apiService.GetAsync<BookingViewModel>($"api/bookings/{bookingId}");
            var model = new ReviewViewModel { BookingId = bookingId };
            
            if (response.Success && response.Data != null)
            {
                model.TripId = response.Data.TripId;
                model.TripRoute = response.Data.RouteName;
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Booking not found.";
                return RedirectToAction("MyBookings", "TicketBooking");
            }

            if (model.TripId <= 0)
            {
                TempData["ErrorMessage"] = "Trip details are missing for this booking.";
                return RedirectToAction("MyBookings", "TicketBooking");
            }

            if (response.Data.Status == "Cancelled")
            {
                TempData["ErrorMessage"] = "Cancelled bookings cannot be reviewed.";
                return RedirectToAction("MyBookings", "TicketBooking");
            }

            if (string.IsNullOrWhiteSpace(model.TripRoute))
            {
                model.TripRoute = "Your Recent Journey";
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Submit(ReviewViewModel model)
        {
            if (!IsCustomer()) return RedirectToAction("LoginCustomer", "Auth");

            if (!ModelState.IsValid) return View(model);

            var response = await _apiService.PostAsync<object>("api/reviews", new
            {
                tripId = model.TripId,
                rating = model.Rating,
                comment = model.Comment
            });
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Thank you for your feedback!";
                return RedirectToAction("MyBookings", "TicketBooking");
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to submit review.");
            return View(model);
        }
        private bool IsAgency() => HttpContext.Session.GetString("UserRole") == "Agency";

        [HttpGet]
        public async Task<IActionResult> MyReviews()
        {
            if (!IsCustomer()) return RedirectToAction("LoginCustomer", "Auth");

            var response = await _apiService.GetAsync<List<ReviewViewModel>>("api/reviews/my");
            return View(response.Data ?? new List<ReviewViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Agency()
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            // Fetch agency summary from API (using a default ID 1 or getting from session if available)
            var response = await _apiService.GetAsync<AgencyReviewSummaryViewModel>("api/reviews/agency/1"); 
            return View(response.Data ?? new AgencyReviewSummaryViewModel());
        }
    }
}
