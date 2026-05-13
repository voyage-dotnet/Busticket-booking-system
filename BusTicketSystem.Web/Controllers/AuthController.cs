using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;

        public AuthController(IAuthService service)
        {
            _service = service;
        }

        [HttpPost("Register-customer")]
        public async Task<IActionResult> RegisterCustomer(RegisterRequestDTO request)
        {
            var result = await _service.RegisterCustomerAsync(request);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("login-customer")]
        public async Task<IActionResult> LoginCustomer(LoginRequestDTO request)
        {
            var result = await _service.LoginCustomerAsync(request);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            
            return StatusCode(result.StatusCode, result);
        }

        // [Authorize(Roles = "Customer")]
        [HttpPatch("Customer-forget-password")]
        public async Task<IActionResult> CustomerForgetPassword(string Email, UpdatePasswordDTO request)
        {
            var result = await _service.ForgetCustomerPasswordAsync(Email, request);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }

            return StatusCode(result.StatusCode, result);
        }
        
        [Authorize(Roles = "Customer")]
        [HttpPatch("Update-customer-email")]
        public async Task<IActionResult> CustomerForgetEmail(string Email, UpdateEmailDTO request)
        {
            var result = await _service.UpdateCustomerEmailAsync(Email, request);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Register-agency")]

        public async Task<IActionResult> RegisterAgency(AgencyRegisterDTO request)
        {
            var result = await _service.RegisterAgencyAsync(request);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }

            return StatusCode(result.StatusCode, result);
        }

        [HttpPost("Login-agency")]

        public async Task<IActionResult> LoginAgency(LoginRequestDTO request)
        {
            var result = await _service.LoginAgencyAsync(request);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }

            return StatusCode(result.StatusCode, result);
        }

        // [Authorize(Roles = "Agency")]
        [HttpPatch("Agency-forget-password")]

        public async Task<IActionResult> AgencyForgetPassword(string Email, UpdatePasswordDTO request)
        {
            var result = await _service.ForgetAgencyPasswordAsync(Email, request);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }

            return StatusCode(result.StatusCode, result);
        }

        [Authorize(Roles = "Agency")]
        [HttpPatch("Update-agency-email")]
        public async Task<IActionResult> AgencyForgetEmail(string Email, UpdateEmailDTO request)
        {
            var result = await _service.UpdateAgencyEmailAsync(Email, request);

            
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }

            return StatusCode(result.StatusCode, result);
        }

    }
}