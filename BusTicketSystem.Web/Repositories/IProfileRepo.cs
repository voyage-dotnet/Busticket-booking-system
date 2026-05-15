using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Repositories
{
    public interface IProfileRepo
    {
        Task<Address> AddCustomerAddressAsync(Address request);

        Task SaveAddressOfCustomerAsync();

        Task<CustomerProfileDTO?> GetCustomerProfile(string Email);
        
    }
}