using System;
using System.Collections.Generic;

namespace BusTicketSystem.Web.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int? TripId { get; set; }

    public int SeatNumber { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Trip? Trip { get; set; }
}
