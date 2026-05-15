using BusTicketSystem.MVC.Services;
using BusTicketSystem.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class TicketBookingController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IConfiguration _configuration;

        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public TicketBookingController(ApiService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            _configuration = configuration;
        }

        private bool IsLoggedIn() => !string.IsNullOrEmpty(HttpContext.Session.GetString("JwtToken"));

        public async Task<IActionResult> MyBookings()
        {
            if (!IsLoggedIn()) return RedirectToAction("LoginCustomer", "Auth");

            var response = await _apiService.GetAsync<List<BookingViewModel>>("api/bookings/my");
            return View(response.Data ?? new List<BookingViewModel>());
        }

        [HttpGet]
        public IActionResult Confirm() => RedirectToAction("Index", "Trips");

        [HttpPost]
        public async Task<IActionResult> Confirm(int tripId, int seatNumber)
        {
            if (!IsLoggedIn()) return RedirectToAction("LoginCustomer", "Auth");

            // 1. Create Booking
            var bookingResponse = await _apiService.PostAsync<BookingViewModel>("api/bookings", new { tripId, seatNumber });
            if (!bookingResponse.Success || bookingResponse.Data == null)
            {
                TempData["ErrorMessage"] = bookingResponse.Message ?? "Failed to initiate booking.";
                return RedirectToAction("Index", "Trips");
            }

            var booking = bookingResponse.Data;

            // 2. Create Razorpay Order
            var orderResponse = await _apiService.PostAsync<RazorpayOrderResponse>("api/payments/razorpay/create-order", new 
            { 
                bookingId = booking.BookingId,
                amount = booking.Fare
            });

            if (!orderResponse.Success || orderResponse.Data == null)
            {
                TempData["ErrorMessage"] = orderResponse.Message ?? "Booking created but failed to initiate payment.";
                return RedirectToAction(nameof(MyBookings));
            }

            // 3. Setup Checkout ViewModel
            var checkoutModel = new RazorpayCheckoutViewModel
            {
                BookingId = booking.BookingId,
                RazorpayOrderId = orderResponse.Data.OrderId,
                AmountInPaise = (int)(booking.Fare * 100),
                RazorpayKey = _configuration["Razorpay:KeyId"] ?? "",
                CustomerName = HttpContext.Session.GetString("UserName") ?? HttpContext.Session.GetString("UserEmail")?.Split('@')[0] ?? "Customer",
                CustomerEmail = HttpContext.Session.GetString("UserEmail") ?? string.Empty,
                CustomerPhone = HttpContext.Session.GetString("UserPhone") ?? string.Empty
            };

            return View("Checkout", checkoutModel);
        }

        [HttpPost]
        public async Task<IActionResult> VerifyPayment(VerifyPaymentViewModel model)
        {
            if (!IsLoggedIn()) return RedirectToAction("LoginCustomer", "Auth");

            var response = await _apiService.PostAsync<object>("api/payments/razorpay/verify", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Payment verified and booking confirmed!";
                return RedirectToAction(nameof(MyBookings));
            }

            TempData["ErrorMessage"] = response.Message ?? "Payment verification failed.";
            return RedirectToAction(nameof(MyBookings));
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("LoginCustomer", "Auth");

            var response = await _apiService.PutAsync<object>($"api/bookings/{id}/cancel", new { });
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Booking cancelled successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = response.Message ?? "Unable to cancel this booking.";
            }

            return RedirectToAction(nameof(MyBookings));
        }
        [HttpGet]
        public async Task<IActionResult> Ticket(int id)
        {
            if (!IsLoggedIn()) return RedirectToAction("LoginCustomer", "Auth");

            var response = await _apiService.GetAsync<BookingViewModel>($"api/bookings/{id}");
            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = "Ticket not found.";
                return RedirectToAction(nameof(MyBookings));
            }

            var model = response.Data;
            
            // Fetch additional trip info (like BoardingPoint and AgencyName)
            var tripResponse = await _apiService.GetAsync<TripDetailResponse>($"api/Trip/{model.TripId}");
            if (tripResponse.Success && tripResponse.Data != null)
            {
                var tripData = tripResponse.Data;
                model.BoardingPoint = tripData.BoardingAddress != null && !string.IsNullOrEmpty(tripData.BoardingAddress.Street)
                    ? $"{tripData.BoardingAddress.Street}, {tripData.BoardingAddress.City}" 
                    : "Main Terminal";
                model.AgencyName = tripData.Agency?.Name ?? "VoyaBus";
                model.ArrivalTime = tripData.ArrivalTime;
            }
            
            return View(model);
        }
    }
}
