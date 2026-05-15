using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services
{
    public interface IAgencyService
    {
        Task<List<AgencyResponseDTO>> GetAllAgenciesAsync();
        Task<AgencyResponseDTO> GetAgencyByIdAsync(int agencyId);
        Task<AgencyResponseDTO> GetMyAgencyAsync(int agencyId);
        Task<AgencyResponseDTO> UpdateMyAgencyAsync(int agencyId, UpdateAgencyRequestDTO dto);

        Task<List<OfficeResponseDTO>> GetMyOfficesAsync(int agencyId);
        Task<OfficeResponseDTO> GetOfficeByIdAsync(int officeId, int agencyId);
        Task<OfficeResponseDTO> CreateOfficeAsync(int agencyId, OfficeCreateDTO dto);
        Task<OfficeResponseDTO> UpdateOfficeAsync(int officeId, int agencyId, OfficeUpdateDTO dto);

        Task<List<BusResponseDTO>> GetBusesByOfficeIdAsync(int officeId, int agencyId);
        Task<BusResponseDTO> CreateBusAsync(int agencyId, CreateBusRequestDTO dto);
        Task<BusResponseDTO> UpdateBusAsync(int busId, int agencyId, UpdateBusRequestDTO dto);

        Task<List<DriverResponseDTO>> GetDriversByOfficeIdAsync(int officeId, int agencyId);
        Task<DriverResponseDTO> CreateDriverAsync(int agencyId, DriverCreateDTO dto);
        Task<DriverResponseDTO> UpdateDriverAsync(int driverId, int agencyId, DriverUpdateDTO dto);

        Task<List<BusResponseDTO>> GetMyBusesAsync(int agencyId);
        Task<List<DriverResponseDTO>> GetMyDriversAsync(int agencyId);
    }
}
