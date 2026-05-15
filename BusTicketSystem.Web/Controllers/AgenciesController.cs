using BusTicketSystem.Web.ResponseWrapper;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Filters;
using BusTicketSystem.Web.Helper;
using BusTicketSystem.Web.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.Web.Controllers
{
    [ApiController]
    [Route("api/agencies")]
    public class AgenciesController : ControllerBase
    {
        private readonly IAgencyService _agencyService;
        private readonly ICurrentUserService _currentUserService;

        public AgenciesController(IAgencyService agencyService, ICurrentUserService currentUserService)
        {
            _agencyService = agencyService;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAgencies()
        {
            var agencies = await _agencyService.GetAllAgenciesAsync();
            return Ok(ApiResponse<List<AgencyResponseDTO>>.SuccessResponse(agencies, "Agencies fetched successfully.", StatusCodes.Status200OK));
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAgencyById(int id)
        {
            var agency = await _agencyService.GetAgencyByIdAsync(id);
            return Ok(ApiResponse<AgencyResponseDTO>.SuccessResponse(agency, "Agency fetched successfully.", StatusCodes.Status200OK));
        }

        [HttpGet("me")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetMyAgency()
        {
            var agencyId = _currentUserService.GetAgencyId();
            var agency = await _agencyService.GetMyAgencyAsync(agencyId);
            return Ok(ApiResponse<AgencyResponseDTO>.SuccessResponse(agency, "Agency profile fetched successfully.", StatusCodes.Status200OK));
        }

        [HttpPut("me")]
        [Authorize(Roles = "Agency")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> UpdateMyAgency([FromBody] UpdateAgencyRequestDTO dto)
        {
            var agencyId = _currentUserService.GetAgencyId();
            var agency = await _agencyService.UpdateMyAgencyAsync(agencyId, dto);
            return Ok(ApiResponse<AgencyResponseDTO>.SuccessResponse(agency, "Agency profile updated successfully.", StatusCodes.Status200OK));
        }

        [HttpGet("me/buses")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetMyBuses()
        {
            var agencyId = _currentUserService.GetAgencyId();
            var buses = await _agencyService.GetMyBusesAsync(agencyId);
            return Ok(ApiResponse<List<BusResponseDTO>>.SuccessResponse(buses, "Buses fetched successfully.", StatusCodes.Status200OK));
        }

        [HttpGet("me/drivers")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetMyDrivers()
        {
            var agencyId = _currentUserService.GetAgencyId();
            var drivers = await _agencyService.GetMyDriversAsync(agencyId);
            return Ok(ApiResponse<List<DriverResponseDTO>>.SuccessResponse(drivers, "Drivers fetched successfully.", StatusCodes.Status200OK));
        }
    }
}
