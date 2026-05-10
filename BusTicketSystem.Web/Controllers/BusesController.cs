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
    public class BusesController : ControllerBase
    {
        private readonly IAgencyService _agencyService;
        private readonly ICurrentUserService _currentUserService;

        public BusesController(IAgencyService agencyService, ICurrentUserService currentUserService)
        {
            _agencyService = agencyService;
            _currentUserService = currentUserService;
        }

        [HttpGet("api/offices/{officeId:int}/buses")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetBusesByOfficeId(int officeId)
        {
            var agencyId = _currentUserService.GetAgencyId();
            var buses = await _agencyService.GetBusesByOfficeIdAsync(officeId, agencyId);
            return Ok(ApiResponse<List<BusResponseDTO>>.SuccessResponse(buses, "Buses fetched successfully.", StatusCodes.Status200OK));
        }

        [HttpPost("api/buses")]
        [Authorize(Roles = "Agency")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> CreateBus([FromBody] CreateBusRequestDTO dto)
        {
            var agencyId = _currentUserService.GetAgencyId();
            var bus = await _agencyService.CreateBusAsync(agencyId, dto);
            return StatusCode(StatusCodes.Status201Created, ApiResponse<BusResponseDTO>.SuccessResponse(bus, "Bus created successfully.", StatusCodes.Status201Created));
        }

        [HttpPut("api/buses/{id:int}")]
        [Authorize(Roles = "Agency")]
        [ServiceFilter(typeof(ValidateModelAttribute))]
        public async Task<IActionResult> UpdateBus(int id, [FromBody] UpdateBusRequestDTO dto)
        {
            var agencyId = _currentUserService.GetAgencyId();
            var bus = await _agencyService.UpdateBusAsync(id, agencyId, dto);
            return Ok(ApiResponse<BusResponseDTO>.SuccessResponse(bus, "Bus updated successfully.", StatusCodes.Status200OK));
        }
    }
}
