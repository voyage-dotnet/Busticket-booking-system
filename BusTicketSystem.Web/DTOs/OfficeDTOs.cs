namespace BusTicketSystem.Web.DTOs;

public class OfficeCreateDTO
{
    public string OfficeMail { get; set; } = null!;
    public string OfficeContactPersonName { get; set; } = null!;
    public string OfficeContactNumber { get; set; } = null!;
    public int OfficeAddressId { get; set; }
}

public class OfficeResponseDTO : OfficeCreateDTO
{
    public int OfficeId { get; set; }
    public int AgencyId { get; set; }
    public string FullAddress { get; set; } = string.Empty;
}

public class OfficeUpdateDTO
{
    public string? OfficeMail { get; set; }
    public string? OfficeContactPersonName { get; set; }
    public string? OfficeContactNumber { get; set; }
    public int? OfficeAddressId { get; set; }
}
