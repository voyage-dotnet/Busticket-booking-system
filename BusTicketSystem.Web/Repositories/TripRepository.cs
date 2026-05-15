using System;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace BusTicketSystem.Web.Repositories;

public class TripRepository : ITripRepository
{
    private readonly BusTicketDbContext _context;
    public TripRepository(BusTicketDbContext context)
    {
        _context = context;
    }

    public async Task<Trip> AddAsync(Trip trip)
    {
        await _context.Trips.AddAsync(trip);
        await _context.SaveChangesAsync();
        return trip;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Trips.AnyAsync(t => t.TripId == id);
    }

    public async Task<Bus?> GetBusByIdAsync(int busId)
    {
        return await _context.Buses.FindAsync(busId);
    }

    public async Task<Address?> GetAddressByIdAsync(int addressId)
    {
        return await _context.Addresses.FindAsync(addressId);
    }

    public async Task<IEnumerable<Trip>> GetAllAsync()
    {
        return await _context.Trips
                             .AsNoTracking()
                             .AsSplitQuery()
                             .Include(t => t.Route)
                             .Include(t => t.Bus)
                                 .ThenInclude(t => t.Office)
                                 .ThenInclude(t => t.Agency)
                             .Include(t => t.Driver1Driver)
                             .ToListAsync();
    }

    public async Task<IEnumerable<Booking>> GetBookingsByTripIdAsync(int tripId)
    {
        return await _context.Bookings
            .Where(b => b.TripId == tripId)
            .ToListAsync();
    }

    public async Task<Trip?> GetByIdAsync(int id)
    {
        return await _context.Trips
            .AsSplitQuery()
            .Include(t => t.Route)
            .Include(t => t.Bus)
                .ThenInclude(b => b.Office)
                    .ThenInclude(o => o.Agency)
            .Include(t => t.Driver1Driver)
            .FirstOrDefaultAsync(t => t.TripId == id);
    }

    public async Task<IEnumerable<Trip>> GetTripsByAgencyAsync(int agencyId)
    {
        return await _context.Trips
            .AsNoTracking()
            .AsSplitQuery()
            .Include(t => t.Route)
            .Include(t => t.Bus)
                .ThenInclude(b => b.Office)
            .Where(t => t.Bus.Office.AgencyId == agencyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Trip>> SearchTripsAsync(string? from, string? to, DateTime? date)
    {
        var query = _context.Trips
            .AsNoTracking()
            .AsSplitQuery()
            .Include(t => t.Route)
            .Include(t => t.Bus)
                .ThenInclude(b => b.Office)
                    .ThenInclude(o => o.Agency)
            .AsQueryable();

        if (!string.IsNullOrEmpty(from))
            query = query.Where(t => t.Route.FromCity == from);

        if (!string.IsNullOrEmpty(to))
            query = query.Where(t => t.Route.ToCity == to);

        if (date.HasValue)
            query = query.Where(t => t.TripDate.Date == date.Value.Date);

        return await query.ToListAsync();
    }

    public async Task<Trip> UpdateAsync(Trip trip)
    {
        _context.Trips.Update(trip);
        await _context.SaveChangesAsync();
        return trip;
    }
}
