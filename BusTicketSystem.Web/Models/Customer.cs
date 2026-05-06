using System;
using System.Collections.Generic;

namespace BusTicketSystem.Web.Models;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public int? AddressId { get; set; }

    public string PasswordHash { get; set; } = null!;

    public virtual Address? Address { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
}
