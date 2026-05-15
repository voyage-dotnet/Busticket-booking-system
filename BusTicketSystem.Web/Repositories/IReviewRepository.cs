using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Repositories
{
    public interface IReviewRepository
    {
        Task<Review> AddAsync(Review review);
        Task<Review?> GetByIdAsync(int reviewId);
        Task<List<Review>> GetByTripIdAsync(int tripId);
        Task<List<Review>> GetByAgencyIdAsync(int agencyId);
        Task<List<Review>> GetByCustomerIdAsync(int customerId);
        Task<bool> HasCompletedBookingAsync(int customerId, int tripId);
        Task<bool> HasAlreadyReviewedAsync(int customerId, int tripId);
        Task<Review> UpdateAsync(Review review);
        Task<int> GetNextReviewIdAsync();
    }
}
