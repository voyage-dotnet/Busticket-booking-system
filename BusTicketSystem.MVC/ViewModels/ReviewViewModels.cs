using System.ComponentModel.DataAnnotations;

namespace BusTicketSystem.MVC.ViewModels
{
    public class ReviewViewModel
    {
        public int BookingId { get; set; }
        public int TripId { get; set; }
        public string TripRoute { get; set; } = string.Empty;

        [Required, Range(1, 5, ErrorMessage = "Please select a rating between 1 and 5 stars")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Please tell us about your experience")]
        [MinLength(10, ErrorMessage = "Comment must be at least 10 characters long")]
        public string Comment { get; set; } = string.Empty;
    }

    public class ReviewResponseViewModel : ReviewViewModel
    {
        public int ReviewId { get; set; }
        public string TripName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public DateTime ReviewDate { get; set; }
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
