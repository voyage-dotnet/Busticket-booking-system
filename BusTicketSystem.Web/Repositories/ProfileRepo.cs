using BusTicketSystem.Web.Models;
using Microsoft.EntityFrameworkCore;
using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Repositories
{
    public class ProfileRepo : IProfileRepo
    {
        
        private readonly BusTicketDbContext _context;

        public ProfileRepo(BusTicketDbContext context)
        {
            _context = context;
        }

        public async Task<Address> AddCustomerAddressAsync(Address request)
        {
            await _context.Addresses.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task SaveAddressOfCustomerAsync ()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<CustomerProfileDTO?> GetCustomerProfile(string Email)
        {
            if (string.IsNullOrEmpty(Email)) return null;

            var profile = await _context.Customers
                .Include(c => c.Address)
                .FirstOrDefaultAsync(c => c.Email != null && c.Email.ToLower() == Email.ToLower());
            
            if (profile == null) return null;

            return new CustomerProfileDTO
            {
                CustomerId = profile.CustomerId,
                Name = profile.Name,
                Email = profile.Email,
                Phone = profile.Phone,

                Address = profile.Address == null ? null : new CustomerAddressRegisterDTO
                {
                    Address1 = profile.Address.Address1,
                    City = profile.Address.City,
                    State = profile.Address.State,
                    ZipCode = profile.Address.ZipCode
                }
            };
        }
    }
}