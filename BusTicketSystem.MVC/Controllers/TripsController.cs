using BusTicketSystem.MVC.ViewModels;
using BusTicketSystem.MVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BusTicketSystem.MVC.Controllers
{
    public class TripsController : Controller
    {
        private readonly ApiService _apiService;

        public TripsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index(string from, string to, string date)
        {
            var endpoint = $"api/Trip/search?from={from}&to={to}&date={date}";
            var response = await _apiService.GetAsync<List<TripSummaryViewModel>>(endpoint, requiresAuth: false);

            var model = new TripSearchPageViewModel
            {
                From = from,
                To = to,
                Date = date,
                Trips = response.Data ?? new List<TripSummaryViewModel>()
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _apiService.GetAsync<TripDetailDto>($"api/Trip/{id}", requiresAuth: false);
            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message ?? "Trip not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(response.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();
            var model = new CreateTripViewModel();
            await PopulateSelectListsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTripViewModel model)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();
            if (!ModelState.IsValid) 
            {
                await PopulateSelectListsAsync(model);
                return View(model);
            }

            var response = await _apiService.PostAsync<TripSummaryViewModel>("api/Trip", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Trip created successfully.";
                return RedirectToAction("Agency", "Dashboard");
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Failed to create trip.");
            await PopulateSelectListsAsync(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();

            var response = await _apiService.GetAsync<TripSummaryViewModel>($"api/Trip/{id}");
            if (!response.Success || response.Data == null)
            {
                TempData["ErrorMessage"] = response.Message ?? "Trip not found.";
                return RedirectToAction("Agency", "Dashboard");
            }

            var trip = response.Data;
            var model = new EditTripViewModel
            {
                TripId = trip.TripId,
                DepartureTime = trip.DepartureTime,
                ArrivalTime = trip.ArrivalTime,
                Fare = trip.Fare,
                Status = trip.Status
            };

            await PopulateSelectListsAsync(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTripViewModel model)
        {
            if (!IsAgencyLoggedIn()) return RedirectToLogin();
            if (!ModelState.IsValid)
            {
                await PopulateSelectListsAsync(model);
                return View(model);
            }

            var response = await _apiService.PutAsync<TripSummaryViewModel>($"api/Trip/{id}", model);
            if (response.Success)
            {
                TempData["SuccessMessage"] = "Trip updated successfully.";
                return RedirectToAction("Agency", "Dashboard");
            }

            ModelState.AddModelError(string.Empty, response.Message ?? "Update failed.");
            await PopulateSelectListsAsync(model);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> SelectSeat(int tripId)
        {
            var tripResponse = await _apiService.GetAsync<TripDetailDto>($"api/Trip/{tripId}", requiresAuth: false);
            if (!tripResponse.Success || tripResponse.Data == null)
            {
                TempData["ErrorMessage"] = tripResponse.Message ?? "Trip not found.";
                return RedirectToAction(nameof(Index));
            }

            var occupiedResponse = await _apiService.GetAsync<List<int>>($"api/Trip/{tripId}/seats", requiresAuth: false);
            
            var model = new TripSeatSelectionViewModel
            {
                Trip = tripResponse.Data,
                OccupiedSeats = occupiedResponse.Data ?? new List<int>()
            };

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Review(int tripId, int seatNumber)
        {
            var tripResponse = await _apiService.GetAsync<TripDetailDto>($"api/Trip/{tripId}", requiresAuth: false);
            if (!tripResponse.Success || tripResponse.Data == null)
            {
                TempData["ErrorMessage"] = "Trip details not found.";
                return RedirectToAction(nameof(Index));
            }

            var model = new ReviewJourneyViewModel
            {
                Trip = tripResponse.Data,
                SeatNumber = seatNumber,
                BaseFare = tripResponse.Data.Fare
            };

            return View(model);
        }

        private async Task PopulateSelectListsAsync(object model)
        {
            var routesRes = await _apiService.GetAsync<List<RouteViewModel>>("api/Routes");
            
            var officesRes = await _apiService.GetAsync<List<OfficeViewModel>>("api/agencies/me/offices");
            var offices = officesRes.Data ?? new List<OfficeViewModel>();

            var busList = new List<SelectListItem>();
            var driverList = new List<SelectListItem>();

            foreach (var office in offices)
            {
                var buses = await _apiService.GetAsync<List<BusViewModel>>($"api/offices/{office.OfficeId}/buses");
                if (buses.Success && buses.Data != null)
                {
                    busList.AddRange(buses.Data.Select(b => new SelectListItem { Value = b.BusId.ToString(), Text = $"{b.RegistrationNumber} ({office.OfficeMail})" }));
                }

                var drivers = await _apiService.GetAsync<List<DriverViewModel>>($"api/offices/{office.OfficeId}/drivers");
                if (drivers.Success && drivers.Data != null)
                {
                    driverList.AddRange(drivers.Data.Select(d => new SelectListItem { Value = d.DriverId.ToString(), Text = $"{d.Name} ({office.OfficeMail})" }));
                }
            }

            var routeList = routesRes.Data?.Select(r => new SelectListItem { Value = r.RouteId.ToString(), Text = $"{r.FromCity} to {r.ToCity}" }) ?? new List<SelectListItem>();

            if (model is CreateTripViewModel createModel)
            {
                createModel.Routes = routeList;
                createModel.Buses = busList;
                createModel.Drivers = driverList;
            }
            else if (model is EditTripViewModel editModel)
            {
                editModel.Routes = routeList;
                editModel.Buses = busList;
                editModel.Drivers = driverList;
            }
        }

        private bool IsAgencyLoggedIn()
        {
            var role = HttpContext.Session.GetString("UserRole");
            var token = HttpContext.Session.GetString("JwtToken");
            return role == "Agency" && !string.IsNullOrEmpty(token);
        }

        private IActionResult RedirectToLogin()
        {
            TempData["ErrorMessage"] = "Please log in as an agency.";
            return RedirectToAction("LoginAgency", "Auth");
        }
    }
}
