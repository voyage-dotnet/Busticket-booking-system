using System.ComponentModel.DataAnnotations;

namespace BusTicketSystem.MVC.ViewModels
{
    public class RouteViewModel
    {
        public int RouteId { get; set; }
        public string FromCity { get; set; } = string.Empty;
        public string ToCity { get; set; } = string.Empty;
        public string? BreakPoints { get; set; }
        public int EstimatedDurationMinutes { get; set; }
    }

    public class CreateRouteViewModel
    {
        [Required, Display(Name = "From City")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City name can only contain letters and spaces")]
        public string FromCity { get; set; } = string.Empty;

        [Required, Display(Name = "To City")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "City name can only contain letters and spaces")]
        public string ToCity { get; set; } = string.Empty;

        [Display(Name = "Break Points (e.g. Nashik, Pune)")]
        public string? BreakPoints { get; set; }

        [Required, Range(1, 10000, ErrorMessage = "Duration must be between 1 and 10,000 minutes")]
        [Display(Name = "Estimated Duration (minutes)")]
        public int EstimatedDurationMinutes { get; set; }
    }
}
