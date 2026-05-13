using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;
using BusTicketSystem.Web.Services;
using Moq;

namespace BusTicketSystem.Tests;

public class ReviewServiceTests
{
    private readonly Mock<IReviewRepository> _repoMock;
    private readonly ReviewService _service;

    public ReviewServiceTests()
    {
        _repoMock = new Mock<IReviewRepository>();
        _service  = new ReviewService(_repoMock.Object);
    }

    [Fact]
    public async Task SubmitReviewAsync_ThrowsBadRequest_WhenNoCompletedBooking()
    {
        _repoMock.Setup(r => r.HasCompletedBookingAsync(1, 10)).ReturnsAsync(false);
        var dto = new SubmitReviewDTO { TripId = 10, Rating = 4, Comment = "Good" };
        await Assert.ThrowsAsync<BadRequestException>(() => _service.SubmitReviewAsync(1, dto));
    }

    [Fact]
    public async Task SubmitReviewAsync_ThrowsBadRequest_WhenAlreadyReviewed()
    {
        _repoMock.Setup(r => r.HasCompletedBookingAsync(1, 10)).ReturnsAsync(true);
        _repoMock.Setup(r => r.HasAlreadyReviewedAsync(1, 10)).ReturnsAsync(true);
        var dto = new SubmitReviewDTO { TripId = 10, Rating = 5, Comment = "Nice!" };
        await Assert.ThrowsAsync<BadRequestException>(() => _service.SubmitReviewAsync(1, dto));
    }

    [Fact]
    public async Task UpdateReviewAsync_ThrowsNotFound_WhenReviewDoesNotExist()
    {
        _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Review?)null);
        var dto = new UpdateReviewDTO { Rating = 3, Comment = "Updated" };
        await Assert.ThrowsAsync<NotFoundException>(() => _service.UpdateReviewAsync(999, 1, dto));
    }

    [Fact]
    public async Task UpdateReviewAsync_ThrowsForbidden_WhenNotOwner()
    {
        var review = new Review { ReviewId = 5, CustomerId = 2, Rating = 4, Comment = "Original" };
        _repoMock.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(review);
        var dto = new UpdateReviewDTO { Rating = 1, Comment = "Hacked!" };
        await Assert.ThrowsAsync<ForbiddenException>(() => _service.UpdateReviewAsync(5, 1, dto));
    }
}
