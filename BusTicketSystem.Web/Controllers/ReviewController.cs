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
        private int GetCustomerId()
        {
            var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (value == null)
                throw new BadRequestException("Customer ID not found in token.");
            return int.Parse(value);
        }
        [HttpPost]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> SubmitReview([FromBody] SubmitReviewDTO dto)
        {
            try
            {
                var result = await _reviewService.SubmitReviewAsync(GetCustomerId(), dto);
                return StatusCode(201, ApiResponse<ReviewResponseDTO>.SuccessResponse(
                result, "Review submitted successfully.", 201));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 400));
            }
        }
        [HttpGet("trip/{tripId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByTrip(int tripId)
        {
            var reviews = await _reviewService.GetReviewsByTripAsync(tripId);
            return Ok(ApiResponse<List<ReviewResponseDTO>>.SuccessResponse(
            reviews, $"Reviews for trip {tripId}."));
        }
        [HttpGet("agency/{agencyId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByAgency(int agencyId)
        {
            try
            {
                var summary = await _reviewService.GetReviewsByAgencyAsync(agencyId);
                return Ok(ApiResponse<AgencyReviewSummaryDTO>.SuccessResponse(
                summary, "Agency review summary fetched."));
            }
            catch (NotFoundException ex) 
            {
                return NotFound(ApiResponse<string>.FailureResponse(ex.Message, statusCode: 404)); 
            }
        }
        [HttpGet("my")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetMyReviews()
        {
            var reviews = await _reviewService.GetMyReviewsAsync(GetCustomerId());
            return Ok(ApiResponse<List<ReviewResponseDTO>>.SuccessResponse(
           reviews, "Your reviews fetched successfully."));
        }
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
