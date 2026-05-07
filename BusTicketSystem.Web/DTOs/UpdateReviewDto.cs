namespace BusTicketSystem.Web.DTOs
{
    public class UpdateReviewDto
    {
        public int Rating { get; set; }          // 1–5
        public string? Comment { get; set; }
    }
}
