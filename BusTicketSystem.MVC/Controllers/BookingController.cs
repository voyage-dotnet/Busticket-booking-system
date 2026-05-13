using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using BusTicketSystem.MVC.ViewModels;
using BusTicketSystem.MVC.Models.Booking;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers;

public class BookingController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _config;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public BookingController(IHttpClientFactory httpClientFactory, IConfiguration config)
    {
        _httpClientFactory = httpClientFactory;
        _config = config;
    }

    private HttpClient CreateApiClient()
    {
        var client = _httpClientFactory.CreateClient("BusTicketApi");
        var token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiU2hhemlhIEFmcmVlbiIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6InNoYXppeWEwMjA1QGdtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL25hbWVpZGVudGlmaWVyIjoiMTczIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQ3VzdG9tZXIiLCJleHAiOjE3Nzg4MzQwOTUsImlzcyI6Ik15U2VydmVyIiwiYXVkIjoiTXlDbGllbnQifQ.RWUe4p27_Uwh1Sj3ye5zEYRtwcW5sVIwI5Y5mc-qJ2o";
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        return client;
    }

    private static async Task<T?> Unwrap<T>(HttpResponseMessage response)
    {
        var json = await response.Content.ReadAsStringAsync();
        var envelope = JsonSerializer.Deserialize<ApiEnvelopeDto<T>>(json, _json);
        return envelope is { Data: not null } ? envelope.Data : default;
    }

    [HttpGet]
    public async Task<IActionResult> MyBookings()
    {
        var client = CreateApiClient();
        var response = await client.GetAsync("api/bookings/my");

        if (!response.IsSuccessStatusCode)
            return View(new BookingListViewModel());

        var dtos = await Unwrap<List<BookingDto>>(response) ?? new();

        var vm = new BookingListViewModel
        {
            Bookings = dtos.Select(MapToItem).ToList()
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Detail(int id)
    {
        var client = CreateApiClient();
        var response = await client.GetAsync($"api/bookings/{id}");

        if (!response.IsSuccessStatusCode)
        {
            TempData["Error"] = "Booking not found.";
            return RedirectToAction(nameof(MyBookings));
        }

        var dto = await Unwrap<BookingDto>(response);
        if (dto is null) return RedirectToAction(nameof(MyBookings));

        var vm = new BookingConfirmationViewModel
        {
            BookingId     = dto.BookingId,
            RouteName     = dto.RouteName,
            DepartureTime = dto.DepartureTime,
            Fare          = dto.Fare,
            SeatNumber    = dto.SeatNumber,
            Status        = dto.Status
        };

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> SelectSeat(int tripId, string routeName,
        DateTime departureTime, decimal fare)
    {
        var client = CreateApiClient();
        var seatsResponse = await client.GetAsync($"api/bookings/available-seats/{tripId}");

        List<int> seats = new();
        if (seatsResponse.IsSuccessStatusCode)
            seats = await Unwrap<List<int>>(seatsResponse) ?? new();

        var tripResponse = await client.GetAsync($"api/Trip/{tripId}");
        int totalSeats = 40;
        if (tripResponse.IsSuccessStatusCode)
        {
            var tripDetail = await Unwrap<TripDetailDto>(tripResponse);
            if (tripDetail != null) totalSeats = tripDetail.TotalSeats;
        }

        var vm = new SeatSelectionViewModel
        {
            TripId         = tripId,
            RouteName      = routeName,
            DepartureTime  = departureTime,
            Fare           = fare,
            TotalSeats     = totalSeats,
            AvailableSeats = seats
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SeatSelectionViewModel form)
    {
        if (!ModelState.IsValid)
            return View("SelectSeat", form);

        var payload = JsonSerializer.Serialize(new
        {
            tripId     = form.TripId,
            seatNumber = form.SelectedSeat
        });

        var client   = CreateApiClient();
        var response = await client.PostAsync("api/bookings",
            new StringContent(payload, Encoding.UTF8, "application/json"));

        if (!response.IsSuccessStatusCode)
        {
            var errJson = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(errJson))
            {
                var errEnv = JsonSerializer.Deserialize<ApiEnvelopeDto<object>>(errJson, _json);
                TempData["Error"] = $"[{(int)response.StatusCode}] {errEnv?.Message ?? "Booking failed."}";
            }
            else
            {
                TempData["Error"] = $"[{(int)response.StatusCode}] Booking failed. Empty response from API.";
            }

            return RedirectToAction(nameof(SelectSeat), new
            {
                tripId        = form.TripId,
                routeName     = form.RouteName,
                departureTime = form.DepartureTime,
                fare          = form.Fare
            });
        }

        var dto = await Unwrap<BookingDto>(response);
        if (dto is null) return RedirectToAction(nameof(MyBookings));

        TempData["Success"] = "Seat booked! Proceed to payment to confirm your trip.";
        return RedirectToAction(nameof(Detail), new { id = dto.BookingId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        var client   = CreateApiClient();
        var response = await client.PutAsync($"api/bookings/{id}/cancel", null);

        TempData[response.IsSuccessStatusCode ? "Success" : "Error"] =
            response.IsSuccessStatusCode
                ? "Your booking has been cancelled."
                : "Unable to cancel this booking.";

        return RedirectToAction(nameof(MyBookings));
    }

    private static BookingItemViewModel MapToItem(BookingDto dto) => new()
    {
        BookingId     = dto.BookingId,
        TripId        = dto.TripId,
        RouteName     = dto.RouteName,
        DepartureTime = dto.DepartureTime,
        Fare          = dto.Fare,
        SeatNumber    = dto.SeatNumber,
        Status        = dto.Status
    };
}