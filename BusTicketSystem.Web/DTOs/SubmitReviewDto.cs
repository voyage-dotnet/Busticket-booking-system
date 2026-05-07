namespace BusTicketSystem.Web.DTOs
{
    public class SubmitReviewDto
    {
        public int TripId { get; set; }
        public int Rating { get; set; }          // 1–5
        public string? Comment { get; set; }
    }
}
