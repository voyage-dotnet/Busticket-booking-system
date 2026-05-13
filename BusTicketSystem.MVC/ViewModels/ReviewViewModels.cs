using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BusTicketSystem.MVC.ViewModels
{
    public class SubmitReviewViewModel
    {
        public int TripId { get; set; }
        
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
        
        public string? Comment { get; set; }
    }

    public class ReviewResponseViewModel : SubmitReviewViewModel
    {
        public int ReviewId { get; set; }
        public string TripName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
    }

    public class UpdateReviewViewModel
    {
        public int ReviewId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rating { get; set; }
        
        public string? Comment { get; set; }
    }

    public class AgencyReviewSummaryViewModel
    {
        public int AgencyId { get; set; }
        public string AgencyName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<ReviewResponseViewModel> Reviews { get; set; } = new();
    }
}
