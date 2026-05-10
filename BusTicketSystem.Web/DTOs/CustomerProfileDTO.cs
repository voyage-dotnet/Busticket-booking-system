using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTicketSystem.Web.DTOs
{
    public class CustomerProfileDTO
    {
        public int CustomerId { get; set; }

        public string? Name { get; set; }

        public string? Email {get; set;}

        public string? Phone { get; set; }

        public CustomerAddressRegisterDTO? Address {get; set;}
    }
}