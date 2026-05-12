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

            var created = await _reviewRepo.AddAsync(review);
            var saved = await _reviewRepo.GetByIdAsync(created.ReviewId);
            return ReviewMapper.ToResponseDTO(saved!);
        }

        public async Task<List<ReviewResponseDTO>> GetReviewsByTripAsync(int tripId)
        {
            var reviews = await _reviewRepo.GetByTripIdAsync(tripId);
            return ReviewMapper.ToResponseDTOList(reviews);
        }

        public async Task<AgencyReviewSummaryDTO> GetReviewsByAgencyAsync(int agencyId)
        {
            var reviews = await _reviewRepo.GetByAgencyIdAsync(agencyId);
            return ReviewMapper.ToAgencySummaryDTO(agencyId, reviews);
        }

        public async Task<List<ReviewResponseDTO>> GetMyReviewsAsync(int customerId)
        {
            var reviews = await _reviewRepo.GetByCustomerIdAsync(customerId);
            return ReviewMapper.ToResponseDTOList(reviews);
        }

        public async Task<ReviewResponseDTO> UpdateReviewAsync(int reviewId, int customerId, UpdateReviewDTO dto)
        {
            var errors = UpdateReviewValidator.Validate(dto);
            if (errors.Any())
                throw new BadRequestException(string.Join(" | ", errors));

            var review = await _reviewRepo.GetByIdAsync(reviewId)
                ?? throw new NotFoundException($"Review with ID {reviewId} not found.");

            if (review.CustomerId != customerId)
                throw new ForbiddenException("You are not authorized to edit this review.");

            review.Rating = dto.Rating;
            review.Comment = dto.Comment;

            var updated = await _reviewRepo.UpdateAsync(review);
            return ReviewMapper.ToResponseDTO(updated);
        }
    }
}
