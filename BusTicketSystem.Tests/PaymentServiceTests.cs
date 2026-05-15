using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;
using BusTicketSystem.Web.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BusTicketSystem.Tests;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _repoMock;
    private readonly Mock<IHttpClientFactory> _httpMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly PaymentService _service;

    public PaymentServiceTests()
    {
        _repoMock   = new Mock<IPaymentRepository>();
        _httpMock   = new Mock<IHttpClientFactory>();
        _configMock = new Mock<IConfiguration>();
        _service    = new PaymentService(_repoMock.Object, _httpMock.Object, _configMock.Object);
    }

    [Fact]
    public async Task GetPaymentByIdAsync_ThrowsNotFound_WhenPaymentMissing()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Payment?)null);
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetPaymentByIdAsync(1, 999));
    }

    [Fact]
    public async Task GetPaymentByIdAsync_ThrowsForbidden_WhenNotOwner()
    {
        var payment = new Payment { PaymentId = 1, CustomerId = 2 };
        _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(payment);

        await Assert.ThrowsAsync<ForbiddenException>(() => _service.GetPaymentByIdAsync(5, 1));
    }

    [Fact]
    public async Task ProcessPaymentAsync_ThrowsNotFound_WhenBookingDoesNotExist()
    {
        _repoMock.Setup(r => r.GetBookingByIdAsync(999)).ReturnsAsync((Booking?)null);

        var dto = new PaymentRequestDTO
        {
            BookingId     = 999,
            Amount        = 500,
            PaymentMethod = "CARD",
            CardNumber    = "1234567890123456",
            CardHolderName = "Test",
            Expiry        = "12/30",
            CVV           = "123"
        };

        await Assert.ThrowsAsync<NotFoundException>(() => _service.ProcessPaymentAsync(1, dto));
    }

    [Fact]
    public async Task ProcessPaymentAsync_ThrowsBadRequest_WhenAlreadyPaid()
    {
        var booking = new Booking
        {
            BookingId = 1, Status = "Booked",
            Trip = new Trip { Fare = 500 }
        };
        var existingPayment = new Payment { PaymentStatus = "Success" };

        _repoMock.Setup(r => r.GetBookingByIdAsync(1)).ReturnsAsync(booking);
        _repoMock.Setup(r => r.GetByBookingIdAsync(1)).ReturnsAsync(existingPayment);

        var dto = new PaymentRequestDTO
        {
            BookingId     = 1,
            Amount        = 500,
            PaymentMethod = "CARD",
            CardNumber    = "1234567890123456",
            CardHolderName = "Test",
            Expiry        = "12/30",
            CVV           = "123"
        };

        await Assert.ThrowsAsync<BadRequestException>(() => _service.ProcessPaymentAsync(1, dto));
    }
}
