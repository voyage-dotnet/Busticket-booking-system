using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Repositories
{
    public interface IAuthRepo
    {
        Task<Customer?> GetCustomerByEmailAsync(string Email);
        Task<Customer?> RegisterCustomerAsync(Customer request);
        Task UpdateCustomerPasswordAsync(Customer request);

        Task<Agency?> GetAgencyByEmailAysnc(string Email);
        Task<Agency?> RegisterAgencyAsync (Agency request);
        Task UpdateAgencyPasswordAsync (Agency agency);
        

    }
}