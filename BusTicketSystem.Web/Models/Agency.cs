using System;
using System.Collections.Generic;

namespace BusTicketSystem.Web.Models;

public partial class Agency
{
    public int AgencyId { get; set; }

    public string Name { get; set; } = null!;

    public string ContactPersonName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public virtual ICollection<AgencyOffice> AgencyOffices { get; set; } = new List<AgencyOffice>();
}
