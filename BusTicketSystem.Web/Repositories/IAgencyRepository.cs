using BusTicketSystem.Web.Models;

namespace BusTicketSystem.Web.Repositories
{
    public interface IAgencyRepository
    {
        Task<List<Agency>> GetAllAgenciesAsync();
        Task<Agency?> GetAgencyByIdAsync(int agencyId);
        Task UpdateAgencyAsync(Agency agency);
        Task<bool> AgencyEmailExistsAsync(string email, int? ignoreAgencyId = null);

        Task<Address> AddAddressAsync(Address address);
        Task<Address?> GetAddressByIdAsync(int addressId);
        Task UpdateAddressAsync(Address address);
        Task<bool> AddressExistsAsync(int addressId);

        Task<List<AgencyOffice>> GetOfficesByAgencyIdAsync(int agencyId);
        Task<AgencyOffice?> GetOfficeByIdAsync(int officeId);
        Task<AgencyOffice> AddOfficeAsync(AgencyOffice office);
        Task UpdateOfficeAsync(AgencyOffice office);

        Task<List<Bus>> GetBusesByOfficeIdAsync(int officeId);
        Task<Bus?> GetBusByIdAsync(int busId);
        Task<Bus> AddBusAsync(Bus bus);
        Task UpdateBusAsync(Bus bus);
        Task<bool> BusRegistrationExistsAsync(string registrationNumber, int? ignoreBusId = null);

        Task<List<Driver>> GetDriversByOfficeIdAsync(int officeId);
        Task<Driver?> GetDriverByIdAsync(int driverId);
        Task<Driver> AddDriverAsync(Driver driver);
        Task UpdateDriverAsync(Driver driver);
        Task<bool> DriverLicenseExistsAsync(string licenseNumber, int? ignoreDriverId = null);

        Task<bool> OfficeBelongsToAgencyAsync(int officeId, int agencyId);
        Task<bool> BusBelongsToAgencyAsync(int busId, int agencyId);
        Task<bool> DriverBelongsToAgencyAsync(int driverId, int agencyId);
    }
}
