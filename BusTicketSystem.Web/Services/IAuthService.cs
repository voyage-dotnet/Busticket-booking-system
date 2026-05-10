using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusTicketSystem.Web.Wrapper;
using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Services
{
    public interface IAuthService
    {
        Task<ApiResponse<RegisterResponseDTO?>> RegisterCustomerAsync (RegisterRequestDTO request);

        Task<ApiResponse<LoginResponseDTO?>> LoginCustomerAsync(LoginRequestDTO request);

        Task<ApiResponse<Object>> ForgetCustomerPasswordAsync (LoginRequestDTO request);

        Task<ApiResponse<RegisterResponseDTO?>> RegisterAgencyAsync (AgencyRegisterDTO request);

        Task<ApiResponse<LoginResponseDTO?>> LoginAgencyAsync (LoginRequestDTO request);

        Task<ApiResponse<Object>> ForgetAgencyPasswordAsync (LoginRequestDTO request);

    }
}