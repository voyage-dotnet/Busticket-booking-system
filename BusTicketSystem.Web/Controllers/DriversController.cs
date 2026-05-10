using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Filters;
using BusTicketSystem.Web.Helper;
using BusTicketSystem.Web.Services;
using BusTicketSystem.Web.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.Web.Controllers
{
    [ApiController]
    public class DriversController : ControllerBase
    {
        private readonly IAgencyService _agencyService;
        private readonly ICurrentUserService _currentUserService;

        public DriversController(IAgencyService agencyService, ICurrentUserService currentUserService)
        {
            _agencyService = agencyService;
            _currentUserService = currentUserService;
        }

        [HttpGet("api/offices/{officeId:int}/drivers")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetDriversByOfficeId(int officeId)
        {
            var agencyId = _currentUserService.GetAgencyId();
            var drivers = await _agencyService.GetDriversByOfficeIdAsync(officeId, agencyId);
            return Ok(ApiResponse<List<DriverResponseDTO>>.SuccessResponse(drivers, "Drivers fetched successfully.", StatusCodes.Status200OK));
        }

        [HttpPost("api/drivers")]
        [Authorize(Roles = "Agency")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> CreateDriver([FromBody] DriverCreateDTO dto)
        {
            var agencyId = _currentUserService.GetAgencyId();
            var driver = await _agencyService.CreateDriverAsync(agencyId, dto);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<DriverResponseDTO>.SuccessResponse(driver, "Driver created successfully.", StatusCodes.Status201Created));
        }

        [HttpPut("api/drivers/{id:int}")]
        [Authorize(Roles = "Agency")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> UpdateDriver(int id, [FromBody] DriverUpdateDTO dto)
        {
            var agencyId = _currentUserService.GetAgencyId();
            var driver = await _agencyService.UpdateDriverAsync(id, agencyId, dto);
            return Ok(ApiResponse<DriverResponseDTO>.SuccessResponse(driver, "Driver updated successfully.", StatusCodes.Status200OK));
        }
    }
}
