namespace BusTicketSystem.Web.DTOs
{
    public class ReviewResponseDto
    {
        public int ReviewId { get; set; }
        public int TripId { get; set; }
        public string TripName { get; set; } = string.Empty;
        public int CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime ReviewDate { get; set; }
    }
}
