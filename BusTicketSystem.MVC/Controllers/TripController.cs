using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using BusTicketSystem.MVC.Models.Trip;

namespace BusTicketSystem.MVC.Controllers;

public class TripController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    private static readonly JsonSerializerOptions _json = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public TripController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Search(string from, string to, DateTime? date)
    {
        var searchDate = date ?? DateTime.Today;

        var client = _httpClientFactory.CreateClient("BusTicketApi");
        var url = $"api/Trip/search?from={Uri.EscapeDataString(from ?? "")}" +
                  $"&to={Uri.EscapeDataString(to ?? "")}" +
                  $"&date={searchDate:yyyy-MM-dd}";

        var response = await client.GetAsync(url);

        List<TripSearchResultDto> results = new();
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var envelope = JsonSerializer.Deserialize<ApiEnvelope<List<TripSearchResultDto>>>(json, _json);
            results = envelope?.Data ?? new();
        }

        var vm = new TripSearchViewModel
        {
            From    = from ?? "",
            To      = to ?? "",
            Date    = searchDate,
            Results = results
        };

        return View(vm);
    }

    private sealed class ApiEnvelope<T>
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}