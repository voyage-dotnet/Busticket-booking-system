using AutoMapper;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Repositories;
using BusTicketSystem.Web.Services;
using Moq;

using RouteModel = BusTicketSystem.Web.Models.Route;

namespace BusTicketSystem.Tests;

public class RouteServiceTests
{
    private readonly Mock<IRouteRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly RouteService _service;

    public RouteServiceTests()
    {
        _repoMock   = new Mock<IRouteRepository>();
        _mapperMock = new Mock<IMapper>();
        _service    = new RouteService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAllRoutesAsync_ReturnsMappedRoutes()
    {
        var routes = new List<RouteModel>
        {
            new() { RouteId = 1, FromCity = "Delhi", ToCity = "Mumbai" },
            new() { RouteId = 2, FromCity = "Pune",  ToCity = "Goa" }
        };
        var dtos = new List<RouteResponseDTO>
        {
            new() { RouteId = 1, FromCity = "Delhi", ToCity = "Mumbai" },
            new() { RouteId = 2, FromCity = "Pune",  ToCity = "Goa" }
        };

        _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(routes);
        _mapperMock.Setup(m => m.Map<IEnumerable<RouteResponseDTO>>(routes)).Returns(dtos);

        var result = await _service.GetAllRoutesAsync();
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetRouteByIdAsync_ThrowsNotFound_WhenRouteDoesNotExist()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((RouteModel?)null);
        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetRouteByIdAsync(999));
    }

    [Fact]
    public async Task UpdateRouteAsync_ThrowsNotFound_WhenRouteDoesNotExist()
    {
        _repoMock.Setup(r => r.GetByIdAsync(100)).ReturnsAsync((RouteModel?)null);
        var dto = new UpdateRouteRequestDTO { EstimatedDurationMinutes = 120 };
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateRouteAsync(100, dto));
    }

    [Fact]
    public async Task CreateRouteAsync_ReturnsMappedDTO_OnSuccess()
    {
        var request = new CreateRouteRequestDTO { FromCity = "Delhi", ToCity = "Jaipur" };
        var entity  = new RouteModel { RouteId = 5, FromCity = "Delhi", ToCity = "Jaipur" };
        var dto     = new RouteResponseDTO { RouteId = 5, FromCity = "Delhi", ToCity = "Jaipur" };

        _mapperMock.Setup(m => m.Map<RouteModel>(request)).Returns(entity);
        _repoMock.Setup(r => r.AddRouteAsync(entity)).ReturnsAsync(entity);
        _mapperMock.Setup(m => m.Map<RouteResponseDTO>(entity)).Returns(dto);

        var result = await _service.CreateRouteAsync(request);

        Assert.NotNull(result);
        Assert.Equal("Delhi", result.FromCity);
        Assert.Equal("Jaipur", result.ToCity);
    }
}
