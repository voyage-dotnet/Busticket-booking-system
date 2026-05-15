using BusTicketSystem.Web.ResponseWrapper;

using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<RegisterResponseDTO?>> RegisterCustomerAsync (RegisterRequestDTO request);

        Task<ApiResponse<LoginResponseDTO?>> LoginCustomerAsync(LoginRequestDTO request);

        Task<ApiResponse<Object>> ForgetCustomerPasswordAsync (string Email, UpdatePasswordDTO request);

        Task<ApiResponse<Object>> UpdateCustomerEmailAsync(string Email, UpdateEmailDTO request);

        Task<ApiResponse<RegisterResponseDTO?>> RegisterAgencyAsync (AgencyRegisterDTO request);

        Task<ApiResponse<LoginResponseDTO?>> LoginAgencyAsync (LoginRequestDTO request);

        Task<ApiResponse<Object>> ForgetAgencyPasswordAsync (string Email, UpdatePasswordDTO request);

        Task<ApiResponse<Object>> UpdateAgencyEmailAsync(string Email, UpdateEmailDTO request);

    }
}