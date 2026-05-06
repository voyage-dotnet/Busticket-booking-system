using System;
using System.Collections.Generic;

namespace BusTicketSystem.Web.Models;

public partial class Route
{
    public int RouteId { get; set; }

    public string FromCity { get; set; } = null!;

    public string ToCity { get; set; } = null!;

    public int? BreakPoints { get; set; }

    public int? Duration { get; set; }

    public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
}
