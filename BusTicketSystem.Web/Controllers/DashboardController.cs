using BusTicketSystem.Web.ResponseWrapper;
﻿using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using BusTicketSystem.Web.Helper;

namespace BusTicketSystem.Web.Controllers
{
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        private readonly ICurrentUserService _currentUserService;

        public DashboardController(IDashboardService dashboardService, ICurrentUserService currentUserService)
        {
            _dashboardService = dashboardService;
            _currentUserService = currentUserService;
        }

        private int GetCustomerId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private int GetAgencyId() => _currentUserService.GetAgencyId();
        [HttpGet("agency/overview")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetAgencyOverview()
        {
            var overview = await _dashboardService.GetAgencyOverviewAsync(GetAgencyId());
            return Ok(ApiResponse<AgencyOverviewDTO>.SuccessResponse(
                overview, "Agency overview fetched successfully."));
        }
        [HttpGet("agency/trips")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetAgencyTripStats()
        {
            var stats = await _dashboardService.GetAgencyTripStatsAsync(GetAgencyId());
            return Ok(ApiResponse<List<AgencyTripStatsDTO>>.SuccessResponse(
                stats, "Agency trip stats fetched successfully."));
        }
        [HttpGet("agency/top-routes")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetAgencyTopRoutes()
        {
            var routes = await _dashboardService.GetAgencyTopRoutesAsync(GetAgencyId());
            return Ok(ApiResponse<List<TopRouteDTO>>.SuccessResponse(
                routes, "Top routes fetched successfully."));
        }
        [HttpGet("customer/overview")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCustomerOverview()
        {
            var overview = await _dashboardService.GetCustomerOverviewAsync(GetCustomerId());
            return Ok(ApiResponse<CustomerOverviewDTO>.SuccessResponse(
                overview, "Customer overview fetched successfully."));
        }
        [HttpGet("public/stats")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPublicStats()
        {
            var stats = await _dashboardService.GetPublicStatsAsync();
            return Ok(ApiResponse<PublicStatsDTO>.SuccessResponse(
                stats, "Public stats fetched successfully."));
        }
    }
}
