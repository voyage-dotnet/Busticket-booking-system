using System;
using System.Collections.Generic;

namespace BusTicketSystem.Web.Models;

public partial class AgencyOffice
{
    public int OfficeId { get; set; }

    public int? AgencyId { get; set; }

    public string? OfficeMail { get; set; }

    public string? OfficeContactPersonName { get; set; }

    public string? OfficeContactNumber { get; set; }

    public int? OfficeAddressId { get; set; }

    public virtual Agency? Agency { get; set; }

    public virtual ICollection<Bus> Buses { get; set; } = new List<Bus>();

    public virtual ICollection<Driver> Drivers { get; set; } = new List<Driver>();

    public virtual Address? OfficeAddress { get; set; }
}
