using BusTicketSystem.Web.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace BusTicketSystem.Web.Repositories
{
    public class ReviewRepository: IReviewRepository
    {
        private readonly BusTicketDbContext _db;

        public ReviewRepository(BusTicketDbContext db)
        {
            _db = db;
        }

        // ─── Add a new review ────────────────────────────────────────────────────

        public async Task<Review> AddAsync(Review review)
        {
            _db.Reviews.Add(review);
            await _db.SaveChangesAsync();
            return review;
        }

        // ─── Get single review by ID (with navigations) ──────────────────────────

        public async Task<Review?> GetByIdAsync(int reviewId)
        {
            return await _db.Reviews
                .Include(r => r.Customer)
                .Include(r => r.Trip).ThenInclude(t => t.Route)
                .FirstOrDefaultAsync(r => r.ReviewId == reviewId);
        }

        // ─── All reviews for a specific trip ─────────────────────────────────────

        public async Task<List<Review>> GetByTripIdAsync(int tripId)
        {
            return await _db.Reviews
                .Where(r => r.TripId == tripId)
                .Include(r => r.Customer)
                .Include(r => r.Trip).ThenInclude(t => t.Route)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        // ─── All reviews for trips belonging to an agency ────────────────────────

        public async Task<List<Review>> GetByAgencyIdAsync(int agencyId)
        {
            return await _db.Reviews
           .Where(r => r.Trip.Bus.Office.AgencyId == agencyId)
           .Include(r => r.Customer)
           .Include(r => r.Trip)
               .ThenInclude(t => t.Route)
           .Include(r => r.Trip)
               .ThenInclude(t => t.Bus)
                   .ThenInclude(b => b.Office)
                       .ThenInclude(o => o.Agency)
           .OrderByDescending(r => r.ReviewDate)
           .ToListAsync();
        }

        // ─── All reviews written by a customer ───────────────────────────────────

        public async Task<List<Review>> GetByCustomerIdAsync(int customerId)
        {
            return await _db.Reviews
                .Where(r => r.CustomerId == customerId)
                .Include(r => r.Customer)
                .Include(r => r.Trip).ThenInclude(t => t.Route)
                .OrderByDescending(r => r.ReviewDate)
                .ToListAsync();
        }

        // ─── Guard: does customer have a completed booking for this trip? ─────────

        public async Task<bool> HasCompletedBookingAsync(int customerId, int tripId)
        {
            return await _db.Payments
           .AnyAsync(p => p.CustomerId == customerId
                       && p.PaymentStatus == "Success"
                       && p.Booking.TripId == tripId
                       && p.Booking.Status == "Booked");
        }

        // ─── Guard: has customer already reviewed this trip? ─────────────────────

        public async Task<bool> HasAlreadyReviewedAsync(int customerId, int tripId)
        {
            return await _db.Reviews
                .AnyAsync(r => r.CustomerId == customerId && r.TripId == tripId);
        }

        // ─── Update an existing review ───────────────────────────────────────────

        public async Task<Review> UpdateAsync(Review review)
        {
            _db.Reviews.Update(review);
            await _db.SaveChangesAsync();
            return (await GetByIdAsync(review.ReviewId))!;
        }
        public async Task<int> GetNextReviewIdAsync()
        {
            var maxId = await _db.Reviews.MaxAsync(r => (int?)r.ReviewId) ?? 0;
            return maxId + 1;
        }
    }
}
