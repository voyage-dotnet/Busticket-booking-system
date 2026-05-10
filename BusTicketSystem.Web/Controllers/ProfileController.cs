using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusTicketSystem.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _service;
        public ProfileController(IProfileService service)
        {
            _service = service;
        }

        [Authorize(Roles = "Customer")]
        [HttpPost("Add-customer-address")]

        public async Task<IActionResult> AddCustomerAddress(string Email, CustomerAddressRegisterDTO request)
        {
            var result = await _service.AddCustomerAddress(Email, request);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }

        [HttpGet("Get-customer-profile")]

        public async Task<IActionResult> GetCustomerProfile(string Email)
        {
            var result = await _service.GetCustomerProfile(Email);

            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result);
            }
            return StatusCode(result.StatusCode, result);
        }
    }
}