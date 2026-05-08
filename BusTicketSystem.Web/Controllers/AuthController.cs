using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Services;
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

        [HttpPatch("Customer-forget-password")]

        public async Task<IActionResult> CustomerForgetPassword(LoginRequestDTO request)
        {
            var result = await _service.ForgetCustomerPasswordAsync(request);

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

        [HttpPatch("Agency-forget-password")]

        public async Task<IActionResult> AgencyForgetPassword(LoginRequestDTO request)
        {
            var result = await _service.ForgetAgencyPasswordAsync(request);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }

            return StatusCode(result.StatusCode, result);
        }

    }
}