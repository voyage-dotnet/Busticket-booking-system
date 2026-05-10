using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Mapping;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;
using BusTicketSystem.Web.Validator;

namespace BusTicketSystem.Web.Services
{
    public class ReviewService: IReviewService
    {
        private readonly IReviewRepository _reviewRepo;

        public ReviewService(IReviewRepository reviewRepo)
        {
            _reviewRepo = reviewRepo;
        }

        // ─── SUBMIT REVIEW ───────────────────────────────────────────────────────

        public async Task<ReviewResponseDTO> SubmitReviewAsync(int customerId, SubmitReviewDTO dto)
        {
            var errors = SubmitReviewValidator.Validate(dto);
            if (errors.Any())
                throw new BadRequestException(string.Join(" | ", errors));

            if (!await _reviewRepo.HasCompletedBookingAsync(customerId, dto.TripId))
                throw new BadRequestException("You can only review a trip after completing a booking.");

            if (await _reviewRepo.HasAlreadyReviewedAsync(customerId, dto.TripId))
                throw new BadRequestException("You have already submitted a review for this trip.");

            var nextId = await _reviewRepo.GetNextReviewIdAsync();
            var review = ReviewMapper.ToEntity(dto, customerId, nextId);


            //var review = new Review
            //{
            //    ReviewId = await _reviewRepo.GetNextReviewIdAsync(),
            //    CustomerId = customerId,
            //    TripId = dto.TripId,
            //    Rating = dto.Rating,
            //    Comment = dto.Comment,
            //    ReviewDate = DateTime.UtcNow
            //};

            var created = await _reviewRepo.AddAsync(review);

            // Re-fetch with navigations to build full response
            var saved = await _reviewRepo.GetByIdAsync(created.ReviewId);
            return ReviewMapper.ToResponseDTO(saved!);
        }

        // ─── GET REVIEWS BY TRIP ─────────────────────────────────────────────────

        public async Task<List<ReviewResponseDTO>> GetReviewsByTripAsync(int tripId)
        {
            var reviews = await _reviewRepo.GetByTripIdAsync(tripId);
            return ReviewMapper.ToResponseDTOList(reviews);
        }

        // ─── GET REVIEWS BY AGENCY ───────────────────────────────────────────────

        public async Task<AgencyReviewSummaryDTO> GetReviewsByAgencyAsync(int agencyId)
        {
            //var reviews = await _reviewRepo.GetByAgencyIdAsync(agencyId);
            //var dtos = reviews.Select(MapToDto).ToList();

            //// Agency name comes from the first review's trip navigation
            //var agencyName = reviews.FirstOrDefault()?.Trip?.Bus.Office.Agency?.Name ?? string.Empty;

            //return new AgencyReviewSummaryDto
            //{
            //    AgencyId = agencyId,
            //    AgencyName = agencyName,
            //    TotalReviews = dtos.Count,
            //    AverageRating = dtos.Count > 0
            //                        ? Math.Round(dtos.Average(r => r.Rating), 2)
            //                        : 0,
            //    Reviews = dtos
            //};
            var reviews = await _reviewRepo.GetByAgencyIdAsync(agencyId);
            return ReviewMapper.ToAgencySummaryDTO(agencyId, reviews);
        }

        // ─── GET MY REVIEWS ──────────────────────────────────────────────────────

        public async Task<List<ReviewResponseDTO>> GetMyReviewsAsync(int customerId)
        {
            var reviews = await _reviewRepo.GetByCustomerIdAsync(customerId);
            return ReviewMapper.ToResponseDTOList(reviews);
        }

        // ─── UPDATE REVIEW ───────────────────────────────────────────────────────

        public async Task<ReviewResponseDTO> UpdateReviewAsync(int reviewId, int customerId, UpdateReviewDTO dto)
        {
            var errors = UpdateReviewValidator.Validate(dto);
            if (errors.Any())
                throw new BadRequestException(string.Join(" | ", errors));

            var review = await _reviewRepo.GetByIdAsync(reviewId)
                ?? throw new NotFoundException("Review", reviewId);

            if (review.CustomerId != customerId)
                throw new ForbiddenException("You are not authorized to edit this review.");

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;

            var updated = await _reviewRepo.UpdateAsync(review);
            return ReviewMapper.ToResponseDTO(updated);
        }
    }
}
