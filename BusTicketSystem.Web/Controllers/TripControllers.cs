using BusTicketSystem.Web.ResponseWrapper;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Helper;
using BusTicketSystem.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripController : ControllerBase
    {
        private readonly ITripService _service;
        private readonly IUserHelper _helper;

        public TripController(ITripService service, IUserHelper helper)
        {
            _service = service;
            _helper = helper;
        }

        [HttpGet]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetAgencyTrips()
        {
            int agencyId = _helper.GetUserId();
            if (agencyId == 0) agencyId = 1;

            var trips = await _service.GetAgencyTripsAsync(agencyId);
            return Ok(ApiResponse<IEnumerable<TripSummaryDTO>>.SuccessResponse(trips));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTripById(int id)
        {
            var trip = await _service.GetTripDetailsAsync(id);
            return Ok(ApiResponse<TripDetailDTO>.SuccessResponse(trip));
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchTrips([FromQuery] string from, [FromQuery] string to, [FromQuery] DateTime date)
        {
            var trips = await _service.SearchTripsAsync(from, to, date);
            return Ok(ApiResponse<IEnumerable<TripSearchResultDTO>>.SuccessResponse(trips));
        }
        [HttpGet("{tripId}/seats")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetSeatLayout(int tripId)
        {
            var layout = await _service.GetSeatLayoutAsync(tripId);
            return Ok(ApiResponse<SeatLayoutDTO>.SuccessResponse(layout));
        }
        [HttpPost]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> CreateTrip([FromBody] CreateTripRequestDTO request)
        {
            var trip = await _service.CreateTripAsync(request);
            return CreatedAtAction(nameof(GetTripById), new { id = trip.TripId }, ApiResponse<TripSummaryDTO>.SuccessResponse(trip, "Trip scheduled successfully", 201));
        }
        [HttpPut("{id}")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> UpdateTrip(int id, [FromBody] UpdateTripRequestDTO request)
        {
            var trip = await _service.UpdateTripAsync(id, request);
            return Ok(ApiResponse<TripSummaryDTO>.SuccessResponse(trip, "Trip updated successfully"));
        }
        [HttpGet("agency/my-trips")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetMyTripsWithOccupancy()
        {
            int agencyId = _helper.GetUserId();
            if (agencyId == 0) agencyId = 1; 

            var trips = await _service.GetMyTripsWithOccupancyAsync(agencyId);
            return Ok(ApiResponse<IEnumerable<MyTripWithOccupancyDTO>>.SuccessResponse(trips));
        }
    }
}
