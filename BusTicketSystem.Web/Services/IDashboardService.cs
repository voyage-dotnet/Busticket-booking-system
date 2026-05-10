using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services
{
    public interface IDashboardService
    {
        Task<AgencyOverviewDTO> GetAgencyOverviewAsync(int agencyId);
        Task<List<AgencyTripStatsDTO>> GetAgencyTripStatsAsync(int agencyId);
        Task<List<TopRouteDTO>> GetAgencyTopRoutesAsync(int agencyId);
        Task<CustomerOverviewDTO> GetCustomerOverviewAsync(int customerId);
        Task<PublicStatsDTO> GetPublicStatsAsync();
    }
}
