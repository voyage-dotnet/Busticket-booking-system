using BusTicketSystem.MVC.ViewModels;
using BusTicketSystem.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApiService _apiService;

        public BookingController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> MyBookings()
        {
            if (!IsCustomerLoggedIn()) return RedirectToLogin();

            var response = await _apiService.GetAsync<List<BookingDto>>("api/bookings/my");
            
            var model = new BookingListViewModel
            {
                Bookings = response.Data?.Select(d => new BookingItemViewModel
                {
                    BookingId = d.BookingId,
                    TripId = d.TripId,
                    RouteName = d.RouteName,
                    DepartureTime = d.DepartureTime,
                    Fare = d.Fare,
                    SeatNumber = d.SeatNumber,
                    Status = d.Status
                }).ToList() ?? new List<BookingItemViewModel>()
            };

            if (!response.Success) TempData["ErrorMessage"] = response.Message;
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (!IsCustomerLoggedIn()) return RedirectToLogin();

            var response = await _apiService.GetAsync<BookingDto>($"api/bookings/{id}");
            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message ?? "Booking not found.";
                return RedirectToAction(nameof(MyBookings));
            }

            var d = response.Data;
            var model = new BookingConfirmationViewModel
            {
                BookingId = d.BookingId,
                RouteName = d.RouteName,
                DepartureTime = d.DepartureTime,
                Fare = d.Fare,
                SeatNumber = d.SeatNumber,
                Status = d.Status
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            if (!IsCustomerLoggedIn()) return RedirectToLogin();

            var response = await _apiService.PutAsync<object>($"api/bookings/{id}/cancel", new { });
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Booking cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Failed to cancel booking.";
            }
            return RedirectToAction(nameof(MyBookings));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Book(int tripId, int selectedSeat)
        {
            if (!IsCustomerLoggedIn()) return RedirectToLogin();

            if (selectedSeat <= 0)
            {
                TempData["ErrorMessage"] = "Please select a seat.";
                return RedirectToAction("Index", "Trips");
            }

            var payload = new { TripId = tripId, SeatNumber = selectedSeat };
            var response = await _apiService.PostAsync<BookingDto>("api/bookings", payload);

            if (response.Success && response.Data != null)
            {
                return RedirectToAction("Pay", "Payment", new { bookingId = response.Data.BookingId });
            }

            TempData["ErrorMessage"] = response.Message ?? "Booking failed.";
            return RedirectToAction("SelectSeat", "Trips", new { tripId });
        }

        private bool IsCustomerLoggedIn()
        {
            var role = HttpContext.Session.GetString("UserRole");
            var token = HttpContext.Session.GetString("JwtToken");
            return role == "Customer" && !string.IsNullOrEmpty(token);
        }

        private IActionResult RedirectToLogin()
        {
            TempData["ErrorMessage"] = "Please log in to continue.";
            return RedirectToAction("LoginCustomer", "Auth");
        }
    }
}
