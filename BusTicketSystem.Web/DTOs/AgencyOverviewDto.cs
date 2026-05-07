namespace BusTicketSystem.Web.DTOs
{
    public class AgencyOverviewDto
    {
        public int TotalTrips { get; set; }
        public int TotalBookings { get; set; }
        public decimal TotalRevenue { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
    }
}
