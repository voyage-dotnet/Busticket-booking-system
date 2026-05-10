using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTicketSystem.Web.DTOs
{
    public class AgencyRegisterDTO
    {
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string ContactPersonName { get; set; } = null!;

        public string Phone { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}