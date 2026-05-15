using BusTicketSystem.Web.ResponseWrapper;
﻿using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Filters;
using BusTicketSystem.Web.Helper;
using BusTicketSystem.Web.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.Web.Controllers
{
    [ApiController]
    public class AgencyOfficesController : ControllerBase
    {
        private readonly IAgencyService _agencyService;
        private readonly ICurrentUserService _currentUserService;

        public AgencyOfficesController(IAgencyService agencyService, ICurrentUserService currentUserService)
        {
            _agencyService = agencyService;
            _currentUserService = currentUserService;
        }

        [HttpGet("api/agencies/me/offices")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetMyOffices()
        {
            var agencyId = _currentUserService.GetAgencyId();
            var offices = await _agencyService.GetMyOfficesAsync(agencyId);
            return Ok(ApiResponse<List<OfficeResponseDTO>>.SuccessResponse(offices, "Offices fetched successfully.", StatusCodes.Status200OK));
        }

        [HttpGet("api/offices/{id:int}")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetOfficeById(int id)
        {
            var agencyId = _currentUserService.GetAgencyId();
            var office = await _agencyService.GetOfficeByIdAsync(id, agencyId);
            return Ok(ApiResponse<OfficeResponseDTO>.SuccessResponse(office, "Office fetched successfully.", StatusCodes.Status200OK));
        }

        [HttpPost("api/agencies/me/offices")]
        [Authorize(Roles = "Agency")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> CreateOffice([FromBody] OfficeCreateDTO dto)
        {
            var agencyId = _currentUserService.GetAgencyId();
            var office = await _agencyService.CreateOfficeAsync(agencyId, dto);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<OfficeResponseDTO>.SuccessResponse(office, "Office created successfully.", StatusCodes.Status201Created));
        }

        [HttpPut("api/offices/{id:int}")]
        [Authorize(Roles = "Agency")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> UpdateOffice(int id, [FromBody] OfficeUpdateDTO dto)
        {
            var agencyId = _currentUserService.GetAgencyId();
            var office = await _agencyService.UpdateOfficeAsync(id, agencyId, dto);
            return Ok(ApiResponse<OfficeResponseDTO>.SuccessResponse(office, "Office updated successfully.", StatusCodes.Status200OK));
        }
    }
}
