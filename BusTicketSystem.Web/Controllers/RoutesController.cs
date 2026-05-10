using BusTicketSystem.Web.ResponseWrapper;
using Microsoft.AspNetCore.Mvc;
using BusTicketSystem.Web.Services;
using BusTicketSystem.Web.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTicketSystem.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoutesController : ControllerBase
{
    private readonly IRouteService _service;
    public RoutesController(IRouteService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllRoutes()
    {
        var route = await _service.GetAllRoutesAsync();
        return Ok(ApiResponse<IEnumerable<RouteResponseDTO>>.SuccessResponse(route));
    }
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetRouteById(int id)
    {
        var route = await _service.GetRouteByIdAsync(id);
        return Ok(ApiResponse<RouteResponseDTO>.SuccessResponse(route));
    }
    [HttpGet("{search}")]
    public async Task<IActionResult> SearchRoutes([FromQuery] string from, [FromQuery] string to)
    {
        var route = await _service.SearchRoutesAsync(from, to);
        return Ok(ApiResponse<IEnumerable<RouteSearchResultDTO>>.SuccessResponse(route));
    }
    [HttpPost]
    [Authorize(Roles = "Agency")]
    public async Task<IActionResult> CreateRoute([FromBody] CreateRouteRequestDTO request)
    {
        var route = await _service.CreateRouteAsync(request);
        return CreatedAtAction(nameof(GetRouteById), new { id = route.RouteId }, ApiResponse<RouteResponseDTO>.SuccessResponse(route, "Created Successfully", 201));
    }
    [HttpPut("{id}")]
    [Authorize(Roles = "Agency")]
    public async Task<IActionResult> UpdateRoutes(int id, [FromBody] UpdateRouteRequestDTO request)
    {
        var route = await _service.UpdateRouteAsync(id, request);
        return Ok(ApiResponse<RouteResponseDTO>.SuccessResponse(route, "route updated Successfully"));
    }
}
