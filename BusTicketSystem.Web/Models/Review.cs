using System;
using System.Collections.Generic;

namespace BusTicketSystem.Web.Models;

public partial class Review
{
    public int ReviewId { get; set; }

    public int CustomerId { get; set; }

    public int TripId { get; set; }

    public int Rating { get; set; }

    public string? Comment { get; set; }

    public DateTime? ReviewDate { get; set; } = DateTime.Now;

    public virtual Customer Customer { get; set; } = null!;

    public virtual Trip Trip { get; set; } = null!;
}
