using System;
using System.Collections.Generic;

namespace BusTicketSystem.Web.Models;

public partial class Trip
{
    public int TripId { get; set; }

    public int RouteId { get; set; }

    public int BusId { get; set; }

    public int BoardingAddressId { get; set; }

    public int DroppingAddressId { get; set; }

    public DateTime DepartureTime { get; set; }

    public DateTime ArrivalTime { get; set; }

    public int Driver1DriverId { get; set; }

    public int Driver2DriverId { get; set; }

    public int AvailableSeats { get; set; }

    public decimal Fare { get; set; }

    public DateTime TripDate { get; set; }

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Bus Bus { get; set; } = null!;

    public virtual Driver Driver1Driver { get; set; } = null!;

    public virtual Driver Driver2Driver { get; set; } = null!;

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    public virtual Route Route { get; set; } = null!;
}
