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


        // GET /api/dashboard/agency/overview
        // Role: Agency — KPIs: trips, bookings, revenue, avg rating
        [HttpGet("agency/overview")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetAgencyOverview()
        {
            var overview = await _dashboardService.GetAgencyOverviewAsync(GetAgencyId());
            return Ok(ApiResponse<AgencyOverviewDTO>.SuccessResponse(
                overview, "Agency overview fetched successfully."));
        }

        // GET /api/dashboard/agency/trips
        // Role: Agency — Trips occupancy % + revenue per trip
        [HttpGet("agency/trips")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetAgencyTripStats()
        {
            var stats = await _dashboardService.GetAgencyTripStatsAsync(GetAgencyId());
            return Ok(ApiResponse<List<AgencyTripStatsDTO>>.SuccessResponse(
                stats, "Agency trip stats fetched successfully."));
        }

        // GET /api/dashboard/agency/top-routes
        // Role: Agency — Top routes by booking count
        [HttpGet("agency/top-routes")]
        [Authorize(Roles = "Agency")]
        public async Task<IActionResult> GetAgencyTopRoutes()
        {
            var routes = await _dashboardService.GetAgencyTopRoutesAsync(GetAgencyId());
            return Ok(ApiResponse<List<TopRouteDTO>>.SuccessResponse(
                routes, "Top routes fetched successfully."));
        }

        // GET /api/dashboard/customer/overview
        // Role: Customer — Trips taken, amount spent, reviews given
        [HttpGet("customer/overview")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCustomerOverview()
        {
            var overview = await _dashboardService.GetCustomerOverviewAsync(GetCustomerId());
            return Ok(ApiResponse<CustomerOverviewDTO>.SuccessResponse(
                overview, "Customer overview fetched successfully."));
        }

        // GET /api/dashboard/public/stats
        // Role: Public — Platform stats (routes, cities, agencies)
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
