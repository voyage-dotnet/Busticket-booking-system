using BusTicketSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BusTicketSystem.Web.Helper;

public class BookingTimeoutHelper : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(1);
    private readonly TimeSpan _timeout  = TimeSpan.FromMinutes(10);
    private static readonly Dictionary<int, DateTime> _pendingTimes = new();

    public BookingTimeoutHelper(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    public static void TrackBooking(int bookingId)
    {
        _pendingTimes[bookingId] = DateTime.Now;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ReleasePendingSeats();
            await Task.Delay(_interval, stoppingToken);
        }
    }

    private async Task ReleasePendingSeats()
    {
        using var scope = _scopeFactory.CreateScope();
        var context     = scope.ServiceProvider
                             .GetRequiredService<BusTicketDbContext>();
        var expiredIds = _pendingTimes
            .Where(x => DateTime.Now - x.Value > _timeout)
            .Select(x => x.Key)
            .ToList();

        if (!expiredIds.Any()) return;

        var expiredBookings = await context.Bookings
            .Where(b => expiredIds.Contains(b.BookingId)
                     && b.Status == "Pending")
            .ToListAsync();

        foreach (var booking in expiredBookings)
        {
            booking.Status = "Available";
            _pendingTimes.Remove(booking.BookingId); 
        }

        if (expiredBookings.Any())
            await context.SaveChangesAsync();
    }
}