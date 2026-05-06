using System;
using System.Collections.Generic;

namespace BusTicketSystem.Web.Models;

public partial class Driver
{
    public int DriverId { get; set; }

    public string LicenseNumber { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int? OfficeId { get; set; }

    public int? AddressId { get; set; }

    public virtual Address? Address { get; set; }

    public virtual AgencyOffice? Office { get; set; }

    public virtual ICollection<Trip> TripDriver1Drivers { get; set; } = new List<Trip>();

    public virtual ICollection<Trip> TripDriver2Drivers { get; set; } = new List<Trip>();
}
