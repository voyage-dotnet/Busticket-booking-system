namespace BusTicketSystem.Web.DTOs;

public class DriverCreateDTO
{
    public string LicenseNumber { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public int OfficeId { get; set; }
    public int AddressId { get; set; }
}

public class DriverResponseDTO
{
    public int DriverId { get; set; }
    public string LicenseNumber { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public int OfficeId { get; set; }
    public int AddressId { get; set; }
}

public class DriverUpdateDTO
{
    public string? LicenseNumber { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public int? OfficeId { get; set; }
    public int? AddressId { get; set; }
}
