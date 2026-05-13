using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.ResponseWrapper;
using BusTicketSystem.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace BusTicketSystem.MVC.Controllers
{
    public class RoutesController : Controller
    {
        private readonly HttpClient _client;

        public RoutesController(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient("BusApi");
        }

        public async Task<IActionResult> Index(string? from, string? to)
        {
            var viewModel = new RouteIndexViewModel { FromSearch = from, ToSearch = to };
            
            ApiResponse<IEnumerable<RouteResponseDTO>>? response;
            if (!string.IsNullOrEmpty(from) || !string.IsNullOrEmpty(to))
            {
                response = await _client.GetFromJsonAsync<ApiResponse<IEnumerable<RouteResponseDTO>>>($"Routes/search?from={from}&to={to}");
            }
            else
            {
                response = await _client.GetFromJsonAsync<ApiResponse<IEnumerable<RouteResponseDTO>>>("Routes");
            }
            
            viewModel.Routes = response?.Data ?? new List<RouteResponseDTO>();
            return View(viewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _client.GetFromJsonAsync<ApiResponse<RouteResponseDTO>>($"Routes/{id}");
            if (response == null || !response.Success) return NotFound();
            
            var viewModel = new RouteDetailsViewModel { Route = response.Data };
            return View(viewModel);
        }

        public IActionResult Create()
        {
            return View(new CreateRouteViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateRouteViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);
            
            var response = await _client.PostAsJsonAsync("Routes", viewModel.Request);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            
            ModelState.AddModelError("", "Failed to create route.");
            return View(viewModel);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _client.GetFromJsonAsync<ApiResponse<RouteResponseDTO>>($"Routes/{id}");
            if (response == null || !response.Success) return NotFound();
            
            var route = response.Data;
            var viewModel = new EditRouteViewModel
            {
                RouteId = id,
                FromCity = route.FromCity,
                ToCity = route.ToCity,
                Request = new UpdateRouteRequestDTO 
                { 
                    BreakPoints = route.BreakPoints,
                    EstimatedDurationMinutes = route.EstimatedDurationMinutes 
                }
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, EditRouteViewModel viewModel)
        {
            if (!ModelState.IsValid) return View(viewModel);
            
            var response = await _client.PutAsJsonAsync($"Routes/{id}", viewModel.Request);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }
            
            ModelState.AddModelError("", "Failed to update route.");
            return View(viewModel);
        }
    }
}
