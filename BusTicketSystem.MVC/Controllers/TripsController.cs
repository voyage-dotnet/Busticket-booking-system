using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.ResponseWrapper;
using BusTicketSystem.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http.Json;

namespace BusTicketSystem.MVC.Controllers
{
    public class TripsController : Controller
    {
        private readonly HttpClient _client;

        public TripsController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("BusApi");
        }

        public async Task<IActionResult> Index(string from, string to, DateTime? date)
        {
            var viewModel = new TripSearchPageViewModel { From = from, To = to, Date = date?.ToString("yyyy-MM-dd") };

            if (!string.IsNullOrEmpty(from) && !string.IsNullOrEmpty(to) && date.HasValue)
            {
                var response = await _client.GetFromJsonAsync<ApiResponse<IEnumerable<TripSearchResultDTO>>>($"Trip/search?from={from}&to={to}&date={date.Value:yyyy-MM-dd}");
                viewModel.Trips = response?.Data ?? new List<TripSearchResultDTO>();
            }

            return View(viewModel);
        }

        public async Task<IActionResult> SelectSeat(int id)
        {
            var tripResponse = await _client.GetFromJsonAsync<ApiResponse<TripDetailDTO>>($"Trip/{id}");
            if (tripResponse == null || !tripResponse.Success) return NotFound();

            var seatResponse = await _client.GetFromJsonAsync<ApiResponse<SeatLayoutDTO>>($"Trip/{id}/seats");
            
            var viewModel = new SeatSelectionViewModel
            {
                Trip = tripResponse.Data,
                SeatLayout = seatResponse?.Data
            };
            
            return View(viewModel);
        }

        public async Task<IActionResult> Review(int tripId, int seatNumber)
        {
            var response = await _client.GetFromJsonAsync<ApiResponse<TripDetailDTO>>($"Trip/{tripId}");
            if (response == null || !response.Success) return NotFound();

            var viewModel = new ReviewJourneyViewModel
            {
                Trip = response.Data,
                SeatNumber = seatNumber,
                BaseFare = response.Data.Fare
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Manage()
        {
            var response = await _client.GetFromJsonAsync<ApiResponse<IEnumerable<MyTripWithOccupancyDTO>>>("Trip/agency/my-trips");
            var viewModel = new AgencyDashboardViewModel
            {
                Trips = response?.Data ?? new List<MyTripWithOccupancyDTO>()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Create()
        {
            var routesResponse = await _client.GetFromJsonAsync<ApiResponse<IEnumerable<RouteResponseDTO>>>("Routes");
            var viewModel = new CreateTripViewModel
            {
                Routes = routesResponse?.Data?.Select(r => new SelectListItem 
                { 
                    Value = r.RouteId.ToString(), 
                    Text = $"{r.FromCity} to {r.ToCity} ({r.EstimatedDurationMinutes} mins)" 
                }) ?? new List<SelectListItem>()
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTripViewModel viewModel)
        {
            if (!ModelState.IsValid) 
            {
                var routesResponse = await _client.GetFromJsonAsync<ApiResponse<IEnumerable<RouteResponseDTO>>>("Routes");
                viewModel.Routes = routesResponse?.Data?.Select(r => new SelectListItem { Value = r.RouteId.ToString(), Text = $"{r.FromCity} to {r.ToCity}" }) ?? new List<SelectListItem>();
                return View(viewModel);
            }
            
            var response = await _client.PostAsJsonAsync("Trip", viewModel.Request);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Manage));
            }
            
            ModelState.AddModelError("", "Failed to create trip.");
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _client.GetFromJsonAsync<ApiResponse<TripDetailDTO>>($"Trip/{id}");
            if (response == null || !response.Success) return NotFound();
            
            var trip = response.Data;
            var viewModel = new EditTripViewModel
            {
                TripId = id,
                Request = new UpdateTripRequestDTO 
                { 
                    Fare = trip.Fare, 
                    DepartureTime = trip.DepartureTime, 
                    ArrivalTime = trip.ArrivalTime,
                    Status = trip.Status
                }
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditTripViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);
            
            var response = await _client.PutAsJsonAsync($"Trip/{id}", viewModel.Request);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Manage));
            }

            ModelState.AddModelError("", "Failed to update trip.");
            return View(viewModel);
        }
    }
}
