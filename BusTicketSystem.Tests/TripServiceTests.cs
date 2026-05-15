using AutoMapper;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;
using BusTicketSystem.Web.Services;
using Moq;

namespace BusTicketSystem.Tests;

public class TripServiceTests
{
    private readonly Mock<ITripRepository> _tripRepoMock;
    private readonly Mock<IRouteRepository> _routeRepoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TripService _service;

    public TripServiceTests()
    {
        _tripRepoMock  = new Mock<ITripRepository>();
        _routeRepoMock = new Mock<IRouteRepository>();
        _mapperMock    = new Mock<IMapper>();
        _service       = new TripService(_tripRepoMock.Object, _routeRepoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateTripAsync_ThrowsNotFound_WhenRouteDoesNotExist()
    {
        _routeRepoMock.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        var request = new CreateTripRequestDTO { RouteId = 999 };

        await Assert.ThrowsAsync<NotFoundException>(() => _service.CreateTripAsync(request));
    }

    [Fact]
    public async Task GetTripDetailsAsync_ThrowsNotFound_WhenTripMissing()
    {
        _tripRepoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Trip?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetTripDetailsAsync(999));
    }

    [Fact]
    public async Task UpdateTripAsync_ThrowsNotFound_WhenTripDoesNotExist()
    {
        _tripRepoMock.Setup(r => r.GetByIdAsync(500)).ReturnsAsync((Trip?)null);

        var request = new UpdateTripRequestDTO { Fare = 200 };

        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateTripAsync(500, request));
    }

    [Fact]
    public async Task GetSeatLayoutAsync_ReturnsLayout_WhenTripExists()
    {
        var trip = new Trip
        {
            TripId = 1,
            AvailableSeats = 2,
            Bus = new Bus { Capacity = 3 }
        };

        var bookings = new List<Booking>
        {
            new Booking { BookingId = 1, SeatNumber = 2, Status = "Booked" }
        };

        _tripRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(trip);
        _tripRepoMock.Setup(r => r.GetBookingsByTripIdAsync(1)).ReturnsAsync(bookings);

        var result = await _service.GetSeatLayoutAsync(1);

        Assert.NotNull(result);
        Assert.Equal(3, result!.TotalSeats);
        Assert.Equal(3, result.Seats.Count);

        Assert.Equal("Available", result.Seats[0].Status);
        Assert.Equal("Booked",    result.Seats[1].Status);
        Assert.Equal("Available", result.Seats[2].Status);
    }
}
