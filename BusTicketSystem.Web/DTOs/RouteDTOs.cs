namespace BusTicketSystem.Web.DTOs;
public class CreateRouteRequestDTO
{
    public string FromCity { get; set; } = null!;
    public string ToCity { get; set; } = null!;
    public string? BreakPoints { get; set; }
    public int EstimatedDurationMinutes { get; set; }
}
public class RouteResponseDTO : CreateRouteRequestDTO
{
    public int RouteId { get; set; }
    public DateTime CreatedAt { get; set; }
}
public class UpdateRouteRequestDTO
{
    public string? BreakPoints { get; set; }
    public int? EstimatedDurationMinutes { get; set; }
}
