using BusTicketSystem.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BusTicketSystem.Web.Repositories
{
    public class AgencyRepository : IAgencyRepository
    {
        private readonly BusTicketDbContext _context;

        public AgencyRepository(BusTicketDbContext context)
        {
            _context = context;
        }

        public async Task<List<Agency>> GetAllAgenciesAsync()
        {
            return await _context.Agencies.OrderBy(a => a.Name).ToListAsync();
        }

        public async Task<Agency?> GetAgencyByIdAsync(int agencyId)
        {
            return await _context.Agencies.FirstOrDefaultAsync(a => a.AgencyId == agencyId);
        }

        public async Task UpdateAgencyAsync(Agency agency)
        {
            _context.Agencies.Update(agency);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AgencyEmailExistsAsync(string email, int? ignoreAgencyId = null)
        {
            var normalisedEmail = email.Trim().ToLower();
            return await _context.Agencies.AnyAsync(a =>
                a.Email.ToLower() == normalisedEmail &&
                (!ignoreAgencyId.HasValue || a.AgencyId != ignoreAgencyId.Value));
        }

        public async Task<Address> AddAddressAsync(Address address)
        {
            _context.Addresses.Add(address);
            await _context.SaveChangesAsync();
            return address;
        }

        public async Task<Address?> GetAddressByIdAsync(int addressId)
        {
            return await _context.Addresses.FirstOrDefaultAsync(a => a.AddressId == addressId);
        }

        public async Task UpdateAddressAsync(Address address)
        {
            _context.Addresses.Update(address);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> AddressExistsAsync(int addressId)
        {
            return await _context.Addresses.AnyAsync(a => a.AddressId == addressId);
        }

        public async Task<List<AgencyOffice>> GetOfficesByAgencyIdAsync(int agencyId)
        {
            return await _context.AgencyOffices
                .Include(o => o.OfficeAddress)
                .Where(o => o.AgencyId == agencyId)
                .OrderBy(o => o.OfficeId)
                .ToListAsync();
        }

        public async Task<AgencyOffice?> GetOfficeByIdAsync(int officeId)
        {
            return await _context.AgencyOffices
                .Include(o => o.OfficeAddress)
                .FirstOrDefaultAsync(o => o.OfficeId == officeId);
        }

        public async Task<AgencyOffice> AddOfficeAsync(AgencyOffice office)
        {
            _context.AgencyOffices.Add(office);
            await _context.SaveChangesAsync();
            return office;
        }

        public async Task UpdateOfficeAsync(AgencyOffice office)
        {
            _context.AgencyOffices.Update(office);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Bus>> GetBusesByOfficeIdAsync(int officeId)
        {
            return await _context.Buses
                .Where(b => b.OfficeId == officeId)
                .OrderBy(b => b.BusId)
                .ToListAsync();
        }

        public async Task<Bus?> GetBusByIdAsync(int busId)
        {
            return await _context.Buses
                .Include(b => b.Office)
                .FirstOrDefaultAsync(b => b.BusId == busId);
        }

        public async Task<Bus> AddBusAsync(Bus bus)
        {
            _context.Buses.Add(bus);
            await _context.SaveChangesAsync();
            return bus;
        }

        public async Task UpdateBusAsync(Bus bus)
        {
            _context.Buses.Update(bus);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> BusRegistrationExistsAsync(string registrationNumber, int? ignoreBusId = null)
        {
            var normalisedRegistration = registrationNumber.Trim().ToLower();
            return await _context.Buses.AnyAsync(b =>
                b.RegistrationNumber.ToLower() == normalisedRegistration &&
                (!ignoreBusId.HasValue || b.BusId != ignoreBusId.Value));
        }

        public async Task<List<Driver>> GetDriversByOfficeIdAsync(int officeId)
        {
            return await _context.Drivers
                .Where(d => d.OfficeId == officeId)
                .OrderBy(d => d.DriverId)
                .ToListAsync();
        }

        public async Task<Driver?> GetDriverByIdAsync(int driverId)
        {
            return await _context.Drivers
                .Include(d => d.Office)
                .FirstOrDefaultAsync(d => d.DriverId == driverId);
        }

        public async Task<Driver> AddDriverAsync(Driver driver)
        {
            _context.Drivers.Add(driver);
            await _context.SaveChangesAsync();
            return driver;
        }

        public async Task UpdateDriverAsync(Driver driver)
        {
            _context.Drivers.Update(driver);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DriverLicenseExistsAsync(string licenseNumber, int? ignoreDriverId = null)
        {
            var normalisedLicense = licenseNumber.Trim().ToLower();
            return await _context.Drivers.AnyAsync(d =>
                d.LicenseNumber.ToLower() == normalisedLicense &&
                (!ignoreDriverId.HasValue || d.DriverId != ignoreDriverId.Value));
        }

        public async Task<bool> OfficeBelongsToAgencyAsync(int officeId, int agencyId)
        {
            return await _context.AgencyOffices.AnyAsync(o => o.OfficeId == officeId && o.AgencyId == agencyId);
        }

        public async Task<bool> BusBelongsToAgencyAsync(int busId, int agencyId)
        {
            return await _context.Buses
                .Include(b => b.Office)
                .AnyAsync(b => b.BusId == busId && b.Office != null && b.Office.AgencyId == agencyId);
        }

        public async Task<bool> DriverBelongsToAgencyAsync(int driverId, int agencyId)
        {
            return await _context.Drivers
                .Include(d => d.Office)
                .AnyAsync(d => d.DriverId == driverId && d.Office != null && d.Office.AgencyId == agencyId);
        }
    }
}
