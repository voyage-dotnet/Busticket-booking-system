using System;
using System.Collections.Generic;

namespace BusTicketSystem.Web.DTOs;

public class SubmitReviewDTO
{
    public int TripId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

public class ReviewResponseDTO
{
    public int TripId { get; set; }
    public string TripName { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime ReviewDate { get; set; }
}

public class UpdateReviewDTO
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

public class AgencyReviewSummaryDTO
{
    public int AgencyId { get; set; }
    public string AgencyName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public List<ReviewResponseDTO> Reviews { get; set; } = new();
}
