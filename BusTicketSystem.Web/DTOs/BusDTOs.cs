namespace BusTicketSystem.Web.DTOs;

public class BusResponseDTO
{
    public int BusId { get; set; }
    public int OfficeId { get; set; }
    public string RegistrationNumber { get; set; } = null!;
    public int Capacity { get; set; }
    public string Type { get; set; } = null!;
}

public class CreateBusRequestDTO
{
    public int OfficeId { get; set; }
    public string RegistrationNumber { get; set; } = null!;
    public int Capacity { get; set; }
    public string Type { get; set; } = null!;
}

public class UpdateBusRequestDTO
{
    public string? RegistrationNumber { get; set; }
    public int? Capacity { get; set; }
    public string? Type { get; set; }
}
