using System;

namespace BusTicketSystem.Web.DTOs;

public class AgencyMiniDTO
{
    public int AgencyId { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
}

public class BusMiniDTO
{
    public int BusId { get; set; }
    public string RegistrationNumber { get; set; }
    public string BusType { get; set; }
    public int TotalCapacity { get; set; }
}

public class DriverMiniDTO
{
    public int DriverId { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
    public string LicenseNumber { get; set; }
}

public class AddressMiniDTO
{
    public int AddressId { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string PinCode { get; set; }
}
