using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services
{
    public interface IReviewService
    {
        Task<ReviewResponseDTO> SubmitReviewAsync(int customerId, SubmitReviewDTO dto);
        Task<List<ReviewResponseDTO>> GetReviewsByTripAsync(int tripId);
        Task<AgencyReviewSummaryDTO> GetReviewsByAgencyAsync(int agencyId);
        Task<List<ReviewResponseDTO>> GetMyReviewsAsync(int customerId);
        Task<ReviewResponseDTO> UpdateReviewAsync(int reviewId, int customerId, UpdateReviewDTO dto);
    }
}
