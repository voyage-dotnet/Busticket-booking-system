namespace BusTicketSystem.Web.DTOs
{
    public class AgencyReviewSummaryDto
    {
        public int AgencyId { get; set; }
        public string AgencyName { get; set; } = string.Empty;
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public List<ReviewResponseDto> Reviews { get; set; } = new();
    }
}
