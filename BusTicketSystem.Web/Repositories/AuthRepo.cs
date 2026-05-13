using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BusTicketSystem.Web.Repositories
{
    public class AuthRepo : IAuthRepo
    {
        private readonly BusTicketDbContext _context;

        public AuthRepo(BusTicketDbContext context)
        {
            _context = context;
        }

        public async Task<Customer?> GetCustomerByEmailAsync(string Email)
        {
            if (string.IsNullOrEmpty(Email)) return null;
            return await _context.Customers.FirstOrDefaultAsync(c => c.Email != null && c.Email.ToLower() == Email.ToLower());
        }
        public async Task<Customer?> RegisterCustomerAsync(Customer request)
        {
            await _context.Customers.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task UpdateCustomerPasswordAsync (Customer request)
        {
            _context.Customers.Update(request);
            await _context.SaveChangesAsync();
  
        }

        public async Task UpdateCustomerEmailAsync(Customer request)
        {
            _context.Customers.Update(request);
            await _context.SaveChangesAsync();
        }

        public async Task<Agency?> GetAgencyByEmailAysnc(string Email)
        {
            if (string.IsNullOrEmpty(Email)) return null;
            return await _context.Agencies.FirstOrDefaultAsync(a => a.Email != null && a.Email.ToLower() == Email.ToLower());
        }

        public async Task<Agency?> RegisterAgencyAsync (Agency request)
        {
            await _context.Agencies.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task UpdateAgencyPasswordAsync (Agency agency)
        {
            _context.Agencies.Update(agency);
            await _context.SaveChangesAsync();
        }

        public async Task UpadateAgencyEmailAsync (Agency request)
        {
            _context.Agencies.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}