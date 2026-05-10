using System;

namespace BusTicketSystem.Web.DTOs;

public class RouteResponseDTO
{
    public int RouteId { get; set; }
    public string FromCity { get; set; }
    public string ToCity { get; set; }
    public string BreakPoints { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class RouteSearchResultDTO
{
    public int RouteId { get; set; }
    public string FromCity { get; set; }
    public string ToCity { get; set; }
    public int EstimatedDurationMinutes { get; set; }
}

public class CreateRouteRequestDTO
{
    public string FromCity { get; set; }
    public string ToCity { get; set; }
    public string? BreakPoints { get; set; }
    public int EstimatedDurationMinutes { get; set; }
}

public class UpdateRouteRequestDTO
{
    public string? BreakPoints { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
}
