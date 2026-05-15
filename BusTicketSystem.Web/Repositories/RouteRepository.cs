using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusTicketSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BusTicketSystem.Web.Repositories;

public class RouteRepository : IRouteRepository
{
    private readonly BusTicketDbContext _context;
    public RouteRepository(BusTicketDbContext context)
    {
        _context = context;
    }
    public async Task<Models.Route> AddRouteAsync(Models.Route route)
    {
        await _context.Routes.AddAsync(route);
        await _context.SaveChangesAsync();
        return route;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Routes.AnyAsync(x => x.RouteId == id);
    }

    public async Task<IEnumerable<Models.Route>> GetAllAsync()
    {
        var route = _context.Routes.ToList();
        return route;
    }

    public async Task<Models.Route?> GetByIdAsync(int id)
    {
        return await _context.Routes.FindAsync(id);
    }

    public async Task<IEnumerable<Models.Route>> SearchRoutesAsync(string fromCity, string toCity)
    {
        return await _context.Routes.Where(r => r.FromCity.Contains(fromCity) && r.ToCity.Contains(toCity)).ToListAsync();
    }

    public async Task<Models.Route> UpdateRouteAsync(Models.Route route)
    {
        _context.Routes.Update(route);
        await _context.SaveChangesAsync();
        return route;
    }
}
