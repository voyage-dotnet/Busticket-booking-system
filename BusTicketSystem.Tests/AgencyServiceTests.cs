using AutoMapper;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;
using BusTicketSystem.Web.Services;
using Moq;

namespace BusTicketSystem.Tests;

public class AgencyServiceTests
{
    private readonly Mock<IAgencyRepository> _repoMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AgencyService _service;

    public AgencyServiceTests()
    {
        _repoMock  = new Mock<IAgencyRepository>();
        _mapperMock = new Mock<IMapper>();
        _service   = new AgencyService(_repoMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task GetAgencyByIdAsync_ReturnsDTO_WhenAgencyExists()
    {
        var agency = new Agency { AgencyId = 1, Name = "Voyage Travels", Email = "info@voyage.com" };
        var expectedDTO = new AgencyResponseDTO { AgencyId = 1, Name = "Voyage Travels" };

        _repoMock.Setup(r => r.GetAgencyByIdAsync(1)).ReturnsAsync(agency);
        _mapperMock.Setup(m => m.Map<AgencyResponseDTO>(agency)).Returns(expectedDTO);

        var result = await _service.GetAgencyByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Voyage Travels", result.Name);
    }

    [Fact]
    public async Task GetAgencyByIdAsync_ThrowsNotFound_WhenAgencyMissing()
    {
        _repoMock.Setup(r => r.GetAgencyByIdAsync(999)).ReturnsAsync((Agency?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _service.GetAgencyByIdAsync(999));
    }

    [Fact]
    public async Task UpdateMyAgencyAsync_ThrowsBadRequest_WhenEmailAlreadyUsed()
    {
        var agency = new Agency { AgencyId = 1, Name = "Old Name", Email = "old@mail.com" };
        var updateDto = new UpdateAgencyRequestDTO { Email = "taken@mail.com" };

        _repoMock.Setup(r => r.GetAgencyByIdAsync(1)).ReturnsAsync(agency);
        _repoMock.Setup(r => r.AgencyEmailExistsAsync("taken@mail.com", 1)).ReturnsAsync(true);

        await Assert.ThrowsAsync<BadRequestException>(
            () => _service.UpdateMyAgencyAsync(1, updateDto));
    }

    [Fact]
    public async Task GetOfficeByIdAsync_ThrowsForbidden_WhenOfficeBelongsToDifferentAgency()
    {
        var office = new AgencyOffice { OfficeId = 10, AgencyId = 2 };

        _repoMock.Setup(r => r.GetOfficeByIdAsync(10)).ReturnsAsync(office);

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _service.GetOfficeByIdAsync(10, agencyId: 1));
    }
}
