using BusTicketSystem.MVC.ViewModels;

using BusTicketSystem.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers;

public class PaymentController : Controller
{
    private readonly ApiService _apiService;

    public PaymentController(ApiService apiService)
    {
        _apiService = apiService;
    }

    [HttpGet]
    public async Task<IActionResult> Pay(int bookingId)
    {
        var response = await _apiService.GetAsync<BookingDto>($"api/bookings/{bookingId}");

        if (!response.Success || response.Data == null)
        {
            TempData["Error"] = response.Message ?? "Booking not found.";
            return RedirectToAction("MyBookings", "Booking");
        }

        var dto = response.Data;
        var vm = new PayViewModel
        {
            BookingId     = dto.BookingId,
            RouteName     = dto.RouteName,
            DepartureTime = dto.DepartureTime,
            SeatNumber    = dto.SeatNumber,
            Fare          = dto.Fare
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessCard(PayViewModel form)
    {
        if (string.IsNullOrWhiteSpace(form.CardHolderName) ||
            string.IsNullOrWhiteSpace(form.CardNumber)     ||
            string.IsNullOrWhiteSpace(form.Expiry)         ||
            string.IsNullOrWhiteSpace(form.CVV))
        {
            ModelState.AddModelError("", "Please fill in all card details.");
            return View("Pay", form);
        }

        var payload = new
        {
            bookingId     = form.BookingId,
            amount        = form.TotalAmount,  
            paymentMethod = "CARD",
            cardHolderName = form.CardHolderName,  
            cardNumber     = form.CardNumber,
            expiry         = form.Expiry,
            cvv            = form.CVV
        };

        var response = await _apiService.PostAsync<PaymentResponseDto>("api/payments", payload);

        if (!response.Success || response.Data == null)
        {
            ModelState.AddModelError("", response.Message ?? "Payment failed. Please try again.");
            return View("Pay", form);
        }

        var result = response.Data;
        return RedirectToAction(nameof(Success), new
        {
            bookingId  = form.BookingId,
            paymentId  = result.PaymentId,
            routeName  = form.RouteName,
            departure  = form.DepartureTime,
            seatNumber = form.SeatNumber,
            amount     = form.TotalAmount,
            method     = "CARD"
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessUpi(PayViewModel form)
    {
        var payload = new
        {
            bookingId     = form.BookingId,
            paymentMethod = "UPI"
        };

        var response = await _apiService.PostAsync<PaymentResponseDto>("api/payments", payload);

        if (!response.Success || response.Data == null)
        {
            ModelState.AddModelError("", response.Message ?? "UPI payment initiation failed.");
            return View("Pay", form);
        }

        var result = response.Data;
        TempData["UpiQR"]      = result.QRCode;
        TempData["UpiUrl"]     = result.UpiUrl;
        TempData["UpiPayId"]   = result.PaymentId;
        TempData["ShowUpiQR"]  = true;

        return RedirectToAction(nameof(Pay), new { bookingId = form.BookingId });
    }

    [HttpGet]
    public IActionResult Success(int bookingId, int paymentId, string routeName,
        DateTime departure, int seatNumber, decimal amount, string method)
    {
        var vm = new PaymentSuccessViewModel
        {
            BookingId     = bookingId,
            PaymentId     = paymentId,
            RouteName     = routeName,
            DepartureTime = departure,
            SeatNumber    = seatNumber,
            Amount        = amount,
            Method        = method
        };

        return View(vm);
    }
}