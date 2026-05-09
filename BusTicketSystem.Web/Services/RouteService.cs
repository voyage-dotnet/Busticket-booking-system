using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Azure;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;

namespace BusTicketSystem.Web.Services;

public class RouteService : IRouteService
{
    private readonly IRouteRepository _repo;
    private readonly IMapper _mapper;

    public RouteService(IRouteRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }
    public async Task<RouteResponseDTO> CreateRouteAsync(CreateRouteRequestDTO request)
    {
        var route = _mapper.Map<Models.Route>(request);
        var creatRoute = await _repo.AddRouteAsync(route);
        return _mapper.Map<RouteResponseDTO>(creatRoute);
    }

    public async Task<IEnumerable<RouteResponseDTO>> GetAllRoutesAsync()
    {
        var route = await _repo.GetAllAsync();
        return _mapper.Map<IEnumerable<RouteResponseDTO>>(route);
    }

    public async Task<RouteResponseDTO?> GetRouteByIdAsync(int id)
    {
        var route = await _repo.GetByIdAsync(id);
        if (route == null) throw new NotFoundException($"Route with {id} not found");
        return _mapper.Map<RouteResponseDTO>(route);
    }

    public async Task<IEnumerable<RouteSearchResultDTO>> SearchRoutesAsync(string fromCity, string toCity)
    {
        var route = await _repo.SearchRoutesAsync(fromCity, toCity);
        return _mapper.Map<IEnumerable<RouteSearchResultDTO>>(route);
    }

    public async Task<RouteResponseDTO> UpdateRouteAsync(int id, UpdateRouteRequestDTO request)
    {
        var route = await _repo.GetByIdAsync(id);
        if (route == null) throw new NotFoundException($"Route with {id} not found");

        _mapper.Map(request, route);
        var updateRoute = await _repo.UpdateRouteAsync(route);
        return _mapper.Map<RouteResponseDTO>(updateRoute);
    }
}
