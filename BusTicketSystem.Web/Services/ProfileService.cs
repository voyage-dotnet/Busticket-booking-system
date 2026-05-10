
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Repositories;

namespace BusTicketSystem.Web.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IProfileRepo _profileRepo;
        private readonly IAuthRepo _authRepo;
        public ProfileService(IProfileRepo profileRepo, IAuthRepo authRepo)
        {
            _profileRepo = profileRepo;
            _authRepo = authRepo;
        }

        public async Task<ApiResponse<Object>> AddCustomerAddress(string Email, CustomerAddressRegisterDTO request)
        {
            // find the customer first 
            var customer = await _authRepo.GetCustomerByEmailAsync(Email);

            if(customer is null)
            {
                return ApiResponse<Object>.FailureResponse("Invalid Profile", new List<string>{""}, 400);
            }

            var customerAddress = new Address();

            customerAddress.Address1 = request.Address1;
            customerAddress.City = request.City;
            customerAddress.State = request.State;
            customerAddress.ZipCode = request.ZipCode;

            var address = await _profileRepo.AddCustomerAddressAsync(customerAddress);

            customer.AddressId = address.AddressId;
            await _profileRepo.SaveAddressOfCustomerAsync();

            return ApiResponse<Object>.SuccessResponse(request, "Address registered Successfully", 200);   
        }

        public async Task<ApiResponse<CustomerProfileDTO>> GetCustomerProfile(string Email)
        {
            var profile = await _profileRepo.GetCustomerProfile(Email);

            if(profile is null)
            {
                return ApiResponse<CustomerProfileDTO>.FailureResponse("Customer not register yet", new List<string>{""}, 400);
            }

            return  ApiResponse<CustomerProfileDTO>.SuccessResponse(profile, "Your Profile Data", 200);   
        }
    }
}