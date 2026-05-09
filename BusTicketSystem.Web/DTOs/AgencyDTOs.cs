namespace BusTicketSystem.Web.DTOs;

public class AgencyResponseDTO
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
}
public class UpdateAgencyRequestDTO
{
    public string? Name { get; set; }
    public string? ContactPersonName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
