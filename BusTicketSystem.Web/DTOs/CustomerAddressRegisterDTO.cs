using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTicketSystem.Web.DTOs
{
    public class CustomerAddressRegisterDTO
    {
        // public int AddressId { get; set; }

        public string Address1 { get; set; } = null!;

        public string City { get; set; } = null!;

        public string State { get; set; } = null!;

        public string ZipCode { get; set; } = null!;

        // public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    }
}