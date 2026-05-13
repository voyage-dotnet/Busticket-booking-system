using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BusTicketSystem.MVC.ViewModels;
using BusTicketSystem.MVC.Models.Booking;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers;

public class PaymentController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public PaymentController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    // ── Helpers (same pattern as BookingController) ───────────────────────────

    private HttpClient CreateApiClient()
    {
        var client = _httpClientFactory.CreateClient("BusTicketApi");
        var token  = HttpContext.Session.GetString("JwtToken");
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static async Task<T?> Unwrap<T>(HttpResponseMessage response)
    {
        var json     = await response.Content.ReadAsStringAsync();
        var envelope = JsonSerializer.Deserialize<ApiEnvelopeDto<T>>(json, _json);
        return envelope is { Data: not null } ? envelope.Data : default;
    }

    // ── STEP 1: Show the payment page ─────────────────────────────────────────
    // GET /Payment/Pay?bookingId=5
    [HttpGet]
    public async Task<IActionResult> Pay(int bookingId)
    {
        var client   = CreateApiClient();
        var response = await client.GetAsync($"api/bookings/{bookingId}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Booking not found.";
            return RedirectToAction("MyBookings", "Booking");
        }

        var dto = await Unwrap<BookingDto>(response);
        if (dto is null) return RedirectToAction("MyBookings", "Booking");

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

    // ── STEP 2a: Process CARD payment ─────────────────────────────────────────
    // POST /Payment/ProcessCard
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessCard(PayViewModel form)
    {
        // Basic card field validation
        if (string.IsNullOrWhiteSpace(form.CardHolderName) ||
            string.IsNullOrWhiteSpace(form.CardNumber)     ||
            string.IsNullOrWhiteSpace(form.Expiry)         ||
            string.IsNullOrWhiteSpace(form.CVV))
        {
            ModelState.AddModelError("", "Please fill in all card details.");
            return View("Pay", form);
        }

        var payload = JsonSerializer.Serialize(new
{
    bookingId     = form.BookingId,
    amount        = form.TotalAmount,  // ✅ API also needs Amount
    paymentMethod = "CARD",
    cardHolderName = form.CardHolderName,  // ✅ flat, no wrapper
    cardNumber     = form.CardNumber,
    expiry         = form.Expiry,
    cvv            = form.CVV
});

        var client   = CreateApiClient();
        var response = await client.PostAsync("api/payments",
            new StringContent(payload, Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var err = await response.Content.ReadAsStringAsync();
            var errEnv = JsonSerializer.Deserialize<ApiEnvelopeDto<object>>(err, _json);
            ModelState.AddModelError("", errEnv?.Message ?? "Payment failed. Please try again.");
            return View("Pay", form);
        }

        var result = await Unwrap<PaymentResponseDto>(response);
        if (result is null)
        {
            ModelState.AddModelError("", "Unexpected response from payment service.");
            return View("Pay", form);
        }

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

    // ── STEP 2b: Process UPI payment ──────────────────────────────────────────
    // POST /Payment/ProcessUpi
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessUpi(PayViewModel form)
    {
        var payload = JsonSerializer.Serialize(new
        {
            bookingId     = form.BookingId,
            paymentMethod = "UPI"
        });

        var client   = CreateApiClient();
        var response = await client.PostAsync("api/payments",
            new StringContent(payload, Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "UPI payment initiation failed.");
            return View("Pay", form);
        }

        var result = await Unwrap<PaymentResponseDto>(response);
        if (result is null)
        {
            ModelState.AddModelError("", "No QR code returned from server.");
            return View("Pay", form);
        }

        // Store QR in TempData so the Pay view can show it
        TempData["UpiQR"]      = result.QRCode;
        TempData["UpiUrl"]     = result.UpiUrl;
        TempData["UpiPayId"]   = result.PaymentId;
        TempData["ShowUpiQR"]  = true;

        return RedirectToAction(nameof(Pay), new { bookingId = form.BookingId });
    }

    // ── STEP 3: Success page ──────────────────────────────────────────────────
    // GET /Payment/Success
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