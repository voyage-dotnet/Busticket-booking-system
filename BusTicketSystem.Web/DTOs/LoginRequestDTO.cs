using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTicketSystem.Web.DTOs
{
    public class LoginRequestDTO
    {
        public string? Email {get; set;}

        public string? Password {get; set;}
    }
}