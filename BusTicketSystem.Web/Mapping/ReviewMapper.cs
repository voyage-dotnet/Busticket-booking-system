using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Mapping
{
    public class ReviewMapper
    {
        public static ReviewResponseDTO ToResponseDTO(Review r)
        {
            return new ReviewResponseDTO
            {
                ReviewId = r.ReviewId,
                TripId = r.TripId,
                TripName = $"{r.Trip?.Route?.FromCity} → {r.Trip?.Route?.ToCity}",
                CustomerId = r.CustomerId,
                CustomerName = r.Customer?.Name ?? string.Empty,
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewDate = r.ReviewDate ?? DateTime.UtcNow
            };
        }
        public static List<ReviewResponseDTO> ToResponseDTOList(List<Review> reviews)
        {
            return reviews.Select(ToResponseDTO).ToList();
        }
        public static Review ToEntity(SubmitReviewDTO dto, int customerId, int nextId)
        {
            return new Review
            {
                ReviewId = nextId,
                CustomerId = customerId,
                TripId = dto.TripId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                ReviewDate = DateTime.UtcNow
            };
        }
        public static AgencyReviewSummaryDTO ToAgencySummaryDTO(int agencyId, List<Review> reviews)
        {
            var dtos = ToResponseDTOList(reviews);
            var agencyName = reviews.FirstOrDefault()
                                ?.Trip?.Bus?.Office?.Agency?.Name
                             ?? string.Empty;

            return new AgencyReviewSummaryDTO
            {
                AgencyId = agencyId,
                AgencyName = agencyName,
                TotalReviews = dtos.Count,
                AverageRating = dtos.Count > 0
                                    ? Math.Round(dtos.Average(r => r.Rating), 2)
                                    : 0,
                Reviews = dtos
            };
        }
    }
}
