using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;
using BusTicketSystem.Web.ResponseWrapper;
using BusTicketSystem.Web.Services;
using Moq;

namespace BusTicketSystem.Tests;

public class BookingServiceTests
{
    private readonly Mock<IBookingRepository> _repoMock;
    private readonly BookingService _service;

    public BookingServiceTests()
    {
        _repoMock = new Mock<IBookingRepository>();
        _service  = new BookingService(_repoMock.Object);
    }

    [Fact]
    public async Task GetBookingByIdAsync_ReturnsFailure_WhenBookingNotFound()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Booking?)null);

        var result = await _service.GetBookingByIdAsync(999);

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task CreateBookingAsync_ReturnsFailure_WhenSeatAlreadyBooked()
    {
        var trip = new Trip
        {
            TripId = 1,
            Bus = new Bus { Capacity = 40 }
        };

        _repoMock.Setup(r => r.GetTripByIdAsync(1)).ReturnsAsync(trip);
        _repoMock.Setup(r => r.IsSeatAlreadyBookedAsync(1, 5)).ReturnsAsync(true);

        var request = new BookingRequestDTO { TripId = 1, SeatNumber = 5 };

        var result = await _service.CreateBookingAsync(customerId: 1, request);

        Assert.False(result.Success);
        Assert.Equal(409, result.StatusCode);
    }

    [Fact]
    public async Task CreateBookingAsync_ReturnsFailure_WhenTripNotFound()
    {
        _repoMock.Setup(r => r.GetTripByIdAsync(999)).ReturnsAsync((Trip?)null);

        var request = new BookingRequestDTO { TripId = 999, SeatNumber = 1 };

        var result = await _service.CreateBookingAsync(customerId: 1, request);

        Assert.False(result.Success);
        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task CancelBookingAsync_ReturnsFailure_WhenAlreadyCancelled()
    {
        var booking = new Booking
        {
            BookingId = 10,
            Status    = "Cancelled",
            Payments  = new List<Payment>
            {
                new Payment { CustomerId = 1 }
            }
        };

        _repoMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(booking);

        var result = await _service.CancelBookingAsync(customerId: 1, bookingId: 10);

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
    }
}
