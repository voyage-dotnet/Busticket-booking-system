using BusTicketSystem.Web.ResponseWrapper;
﻿using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Exceptions;
using BusTicketSystem.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BusTicketSystem.Web.Controllers
{
    [Route("api/reviews")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        // ─── Helper to extract CustomerId from JWT ───────────────────────────────
        //private int GetCustomerId() =>
        //    int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        private int GetCustomerId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (value == null)
                throw new BadRequestException("Customer ID not found in token.");
            return int.Parse(value);
        }

        //[HttpGet("debug-claims")]
        //[Authorize]
        //public IActionResult DebugClaims()
        //{
        //    var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
        //    return Ok(claims);
        //}

        // POST /api/reviews
        // Role: Customer  — Submit review (requires completed booking)
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> SubmitReview([FromBody] SubmitReviewDTO dto)
        {
            try
            {
                var result = await _reviewService.SubmitReviewAsync(GetCustomerId(), dto);
                //return CreatedAtAction(nameof(GetReviewsByTrip),
                //    new { tripId = result.TripId }, result);
                return StatusCode(201, ApiResponse<ReviewResponseDTO>.SuccessResponse(
                result, "Review submitted successfully.", 201));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 400));
            }
        }

        // GET /api/reviews/trip/{tripId}
        // Role: Public  — All reviews for a specific trip
        [HttpGet("trip/{tripId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByTrip(int tripId)
        {
            var reviews = await _reviewService.GetReviewsByTripAsync(tripId);
            //return Ok(reviews);
            return Ok(ApiResponse<List<ReviewResponseDTO>>.SuccessResponse(
            reviews, $"Reviews for trip {tripId}."));
        }

        // GET /api/reviews/agency/{agencyId}
        // Role: Public  — Agency reviews with avg rating
        [HttpGet("agency/{agencyId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByAgency(int agencyId)
        {
            try
            {
                var summary = await _reviewService.GetReviewsByAgencyAsync(agencyId);
                //return Ok(summary);
                return Ok(ApiResponse<AgencyReviewSummaryDTO>.SuccessResponse(
                summary, "Agency review summary fetched."));
            }
            catch (NotFoundException ex) 
            {
                return NotFound(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 404)); 
            }
        }

        // GET /api/reviews/my
        // Role: Customer  — Reviews by the logged-in customer
        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyReviews()
        {
            var reviews = await _reviewService.GetMyReviewsAsync(GetCustomerId());
            return Ok(ApiResponse<List<ReviewResponseDTO>>.SuccessResponse(
           reviews, "Your reviews fetched successfully."));
        }

        // PUT /api/reviews/{id}
        // Role: Customer  — Edit own review (rating / comment)
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateReview(int id, [FromBody] UpdateReviewDTO dto)
        {
            try
            {
                var updated = await _reviewService.UpdateReviewAsync(id, GetCustomerId(), dto);
                return Ok(ApiResponse<ReviewResponseDTO>.SuccessResponse(
                updated, "Review updated successfully."));
            }
            catch (NotFoundException ex)
            {
                return NotFound(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 404));
            }
            catch (ForbiddenException ex)
            {
                return StatusCode(403, ApiResponse<string>.FailureResponse(ex.Message, statusCode: 403));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 400));
            }
        }
    }
}
