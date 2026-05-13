using System;

namespace BusTicketSystem.Web.DTOs;

public class AgencyMiniDTO
{
    public int AgencyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class BusMiniDTO
{
    public int BusId { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;
    public string BusType { get; set; } = string.Empty;
    public int TotalCapacity { get; set; }
}

public class DriverMiniDTO
{
    public int DriverId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
}

public class AddressMiniDTO
{
    public int AddressId { get; set; }
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string PinCode { get; set; } = string.Empty;
}
