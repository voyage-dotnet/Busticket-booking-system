using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Helper;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;
using BusTicketSystem.Web.Services;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BusTicketSystem.Tests;

public class AuthServiceTests
{
    private readonly Mock<IAuthRepo> _repoMock;
    private readonly Mock<IConfiguration> _configMock;
    private readonly Mock<GenerateJwtToken> _tokenMock;
    private readonly AuthService _service;

    public AuthServiceTests()
    {
        _repoMock   = new Mock<IAuthRepo>();
        _configMock = new Mock<IConfiguration>();
        _tokenMock = new Mock<GenerateJwtToken>(_configMock.Object);
        _service = new AuthService(_repoMock.Object, _configMock.Object, _tokenMock.Object);
    }

    [Fact]
    public async Task RegisterCustomerAsync_ReturnsSuccess_WhenEmailIsNew()
    {
        var request = new RegisterRequestDTO
        {
            Name     = "Rahul",
            Email    = "rahul@test.com",
            Phone    = "9876543210",
            Password = "Pass@123"
        };

        _repoMock.Setup(r => r.GetCustomerByEmailAsync("rahul@test.com"))
                 .ReturnsAsync((Customer?)null);

        _repoMock.Setup(r => r.RegisterCustomerAsync(It.IsAny<Customer>()))
                 .ReturnsAsync(new Customer
                 {
                     CustomerId   = 1,
                     Name         = "Rahul",
                     Email        = "rahul@test.com",
                     Phone        = "9876543210",
                     PasswordHash = "hashed"
                 });

        var result = await _service.RegisterCustomerAsync(request);

        Assert.True(result.Success);
        Assert.Equal("Rahul", result.Data!.Name);
    }

    [Fact]
    public async Task RegisterCustomerAsync_ReturnsFailure_WhenEmailAlreadyExists()
    {
        var existing = new Customer { CustomerId = 1, Email = "rahul@test.com" };
        _repoMock.Setup(r => r.GetCustomerByEmailAsync("rahul@test.com"))
                 .ReturnsAsync(existing);

        var request = new RegisterRequestDTO
        {
            Name = "Rahul", Email = "rahul@test.com",
            Phone = "9876543210", Password = "Pass@123"
        };

        var result = await _service.RegisterCustomerAsync(request);

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
    }

    [Fact]
    public async Task LoginCustomerAsync_ReturnsFailure_WhenCustomerNotFound()
    {
        _repoMock.Setup(r => r.GetCustomerByEmailAsync("unknown@test.com"))
                 .ReturnsAsync((Customer?)null);

        var request = new LoginRequestDTO { Email = "unknown@test.com", Password = "any" };

        var result = await _service.LoginCustomerAsync(request);

        Assert.False(result.Success);
        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task RegisterAgencyAsync_ReturnsFailure_WhenAgencyAlreadyExists()
    {
        var existing = new Agency { AgencyId = 1, Email = "agency@test.com" };
        _repoMock.Setup(r => r.GetAgencyByEmailAysnc("agency@test.com"))
                 .ReturnsAsync(existing);

        var request = new AgencyRegisterDTO
        {
            Name = "TestAgency", Email = "agency@test.com",
            ContactPersonName = "Person", Phone = "1234567890",
            Password = "Pass@123"
        };

        var result = await _service.RegisterAgencyAsync(request);

        Assert.False(result.Success);
        Assert.Equal(400, result.StatusCode);
    }
}
