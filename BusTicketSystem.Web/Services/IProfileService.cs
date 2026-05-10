
using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services
{
    public interface IProfileService
    {
        Task<ApiResponse<Object>> AddCustomerAddress(string Email, CustomerAddressRegisterDTO request);

        Task<ApiResponse<CustomerProfileDTO>> GetCustomerProfile(string Email);

        
    }
}