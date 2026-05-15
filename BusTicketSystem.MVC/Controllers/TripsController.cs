using BusTicketSystem.MVC.Services;
using BusTicketSystem.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.MVC.Controllers
{
    public class TripsController : Controller
    {
        private readonly ApiService _apiService;


        public TripsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private bool IsAgency() => HttpContext.Session.GetString("UserRole") == "Agency";

        // Bug #8 fix: explicit [HttpGet] route for the agency trip management list
        [HttpGet]
        public async Task<IActionResult> Agency()
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            var response = await _apiService.GetAsync<List<TripSummaryViewModel>>("api/Trip");
            return View(response.Data ?? new List<TripSummaryViewModel>());
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            ViewBag.Routes = (await _apiService.GetAsync<List<RouteViewModel>>("api/routes")).Data;
            ViewBag.Buses = (await _apiService.GetAsync<List<BusViewModel>>("api/agencies/me/buses")).Data;
            ViewBag.Drivers = (await _apiService.GetAsync<List<DriverViewModel>>("api/agencies/me/drivers")).Data;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTripViewModel model)
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            if (!ModelState.IsValid)
            {
                ViewBag.Routes = (await _apiService.GetAsync<List<RouteViewModel>>("api/routes")).Data;
                ViewBag.Buses = (await _apiService.GetAsync<List<BusViewModel>>("api/agencies/me/buses")).Data;
                ViewBag.Drivers = (await _apiService.GetAsync<List<DriverViewModel>>("api/agencies/me/drivers")).Data;
                return View(model);
            }

            var response = await _apiService.PostAsync<object>("api/Trip", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Trip scheduled successfully!";
                return RedirectToAction(nameof(Agency));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to schedule trip.");
            return View(model);
        }

        // Bug #5 fix: no GET api/buses/{id} endpoint exists; fetch all buses and find by Id
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            var response = await _apiService.GetAsync<CreateTripViewModel>($"api/Trip/{id}");
            if (response.Success && response.Data != null)
            {
                ViewBag.Routes = (await _apiService.GetAsync<List<RouteViewModel>>("api/routes")).Data;
                ViewBag.Buses = (await _apiService.GetAsync<List<BusViewModel>>("api/agencies/me/buses")).Data;
                ViewBag.Drivers = (await _apiService.GetAsync<List<DriverViewModel>>("api/agencies/me/drivers")).Data;
                return View(response.Data);
            }

            TempData["ErrorMessage"] = "Trip not found.";
            return RedirectToAction(nameof(Agency));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CreateTripViewModel model)
        {
            if (!IsAgency()) return RedirectToAction("LoginAgency", "Auth");

            if (!ModelState.IsValid)
            {
                ViewBag.Routes = (await _apiService.GetAsync<List<RouteViewModel>>("api/routes")).Data;
                ViewBag.Buses = (await _apiService.GetAsync<List<BusViewModel>>("api/agencies/me/buses")).Data;
                ViewBag.Drivers = (await _apiService.GetAsync<List<DriverViewModel>>("api/agencies/me/drivers")).Data;
                return View(model);
            }

            var response = await _apiService.PutAsync<object>($"api/Trip/{id}", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Trip updated successfully!";
                return RedirectToAction(nameof(Agency));
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to update trip.");
            return View(model);
        }

        // Bug #8 fix: customer-facing search uses Index — kept distinct from Agency()
        [HttpGet]
        public async Task<IActionResult> Index(string? from, string? to, string? date)
        {
            var endpoint = $"api/Trip/search?from={from}&to={to}&date={date}";
            var response = await _apiService.GetAsync<List<TripSummaryViewModel>>(endpoint);

            var model = new TripSearchPageViewModel
            {
                From = from,
                To = to,
                Date = date,
                Trips = response.Data ?? new List<TripSummaryViewModel>()
            };

            return View(model);
        }

        public async Task<IActionResult> SelectSeat(int tripId)
        {
            var tripResponse = await _apiService.GetAsync<TripDetailResponse>($"api/Trip/{tripId}");
            if (!tripResponse.Success || tripResponse.Data == null)
            {
                TempData["ErrorMessage"] = "Trip not found.";
                return RedirectToAction(nameof(Index));
            }

            var occupiedSeatsResponse = await _apiService.GetAsync<List<int>>($"api/bookings/booked-seats/{tripId}");

            // Map strongly typed response to ViewModel to get BoardingAddress
            var tripData = tripResponse.Data;
            var model = new TripSeatSelectionViewModel
            {
                Trip = new TripSummaryViewModel
                {
                    TripId = tripData.TripId,
                    FromCity = tripData.FromCity,
                    ToCity = tripData.ToCity,
                    DepartureTime = tripData.DepartureTime,
                    ArrivalTime = tripData.ArrivalTime,
                    Fare = tripData.Fare,
                    TotalCapacity = tripData.TotalSeats,
                    AvailableSeats = tripData.AvailableSeats,
                    Status = tripData.Status,
                    BusType = tripData.Bus?.BusType ?? "Standard",
                    AgencyName = tripData.Agency?.Name ?? "VoyaBus",
                    BoardingPoint = tripData.BoardingAddress != null && !string.IsNullOrEmpty(tripData.BoardingAddress.Street)
                        ? $"{tripData.BoardingAddress.Street}, {tripData.BoardingAddress.City}"
                        : "Main Terminal"
                },
                OccupiedSeats = occupiedSeatsResponse.Data ?? new List<int>()
            };

            return View(model);
        }

        public async Task<IActionResult> Review(int tripId, int seatNumber)
        {
            var tripResponse = await _apiService.GetAsync<TripDetailResponse>($"api/Trip/{tripId}");
            if (!tripResponse.Success || tripResponse.Data == null)
            {
                TempData["ErrorMessage"] = "Trip details not found.";
                return RedirectToAction(nameof(Index));
            }

            var tripData = tripResponse.Data;
            var model = new ReviewJourneyViewModel
            {
                Trip = new TripSummaryViewModel
                {
                    TripId = tripData.TripId,
                    FromCity = tripData.FromCity,
                    ToCity = tripData.ToCity,
                    DepartureTime = tripData.DepartureTime,
                    ArrivalTime = tripData.ArrivalTime,
                    Fare = tripData.Fare,
                    TotalCapacity = tripData.TotalSeats,
                    AvailableSeats = tripData.AvailableSeats,
                    Status = tripData.Status,
                    BusType = tripData.Bus?.BusType ?? "Standard",
                    AgencyName = tripData.Agency?.Name ?? "VoyaBus",
                    BoardingPoint = tripData.BoardingAddress != null && !string.IsNullOrEmpty(tripData.BoardingAddress.Street)
                        ? $"{tripData.BoardingAddress.Street}, {tripData.BoardingAddress.City}"
                        : "Main Terminal"
                },
                SeatNumber = seatNumber,
                BaseFare = tripData.Fare
            };

            return View(model);
        }
    }
}
