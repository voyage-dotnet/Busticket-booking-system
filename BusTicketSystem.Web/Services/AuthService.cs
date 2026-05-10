
using BusTicketSystem.Web.DTOs;
using BusTicketSystem.Web.Models;
using BusTicketSystem.Web.Repositories;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Identity;
using BusTicketSystem.Web.Helper;


namespace BusTicketSystem.Web.Services
{
    public class AuthService : IAuthService
    {
        
        private readonly IAuthRepo _repo;
        private readonly IConfiguration _config;
        private readonly GenerateJwtToken _tokenhelper;


        public AuthService( IAuthRepo repo, IConfiguration config, GenerateJwtToken tokenhelper)
        {   
            _repo = repo;
            _config = config;
            _tokenhelper = tokenhelper;
        }

        public async Task<ApiResponse<RegisterResponseDTO?>> RegisterCustomerAsync(RegisterRequestDTO request)
        {
            // check if customer already exist
            var customer = await _repo.GetCustomerByEmailAsync(request.Email);

            if(customer != null)
            {
                return ApiResponse<RegisterResponseDTO?>.FailureResponse("User Already Exist", new List<string>{"Email is already register"}, 400);
            }

            // this is new customer
            var NewCustomer = new Customer();

            NewCustomer.Name = request.Name;

            NewCustomer.Email = request.Email.ToLower();

            NewCustomer.Phone = request.Phone;

            var passwardhash = new PasswordHasher<Customer>().HashPassword(NewCustomer, request.Password);

            NewCustomer.PasswordHash = passwardhash;

            var saved = await _repo.RegisterCustomerAsync(NewCustomer);

            var response = new RegisterResponseDTO{
                Name = saved.Name,
                Email = saved.Email,
                Phone = saved.Phone
            };

            return ApiResponse<RegisterResponseDTO?>.SuccessResponse(response, "Customer Register Successfully", 200);  
        }

        public async Task<ApiResponse<LoginResponseDTO?>> LoginCustomerAsync(LoginRequestDTO request)
        {
            // check if new customer without register
            var customer = await _repo.GetCustomerByEmailAsync(request.Email);

            if(customer is null)
            {
                return ApiResponse<LoginResponseDTO?>.FailureResponse("User not exist", new List<string>{$"{customer}"}, 401);
            }

            // check password is correct or not
            var result = new PasswordHasher<Customer>().VerifyHashedPassword(customer, customer.PasswordHash, request.Password);

            if(result == PasswordVerificationResult.Failed)
            {
                return ApiResponse<LoginResponseDTO?>.FailureResponse("Invalid Credential", new List<string> {$"{result}"}, 401);
            }

            var token = _tokenhelper.GenerateToken(customer.Name, customer.Email, customer.CustomerId, "Customer");

            var response = new LoginResponseDTO{
                Token = token
            };

            return ApiResponse<LoginResponseDTO?>.SuccessResponse(response,"Login Successfully");
        } 

        public async Task<ApiResponse<Object>> ForgetCustomerPasswordAsync (string Email, UpdatePasswordDTO request)
        {
            // check if customer have an account
            
            var existCustomer = await _repo.GetCustomerByEmailAsync(Email);

            if(existCustomer is null)
            {
                return ApiResponse<Object>.FailureResponse("Customer not registered", new List<string> {$"{existCustomer}"}, 400);
            }

            var passwardhash = new PasswordHasher<Customer>().HashPassword(existCustomer, request.Password);
            existCustomer.PasswordHash = passwardhash;

            await _repo.UpdateCustomerPasswordAsync(existCustomer);
            return ApiResponse<Object>.SuccessResponse(null, "Password Update Successfully", 200);
        }

        public async Task<ApiResponse<Object>> UpdateCustomerEmailAsync(string Email, UpdateEmailDTO request)
        {
            var ExistCustomer = await _repo.GetCustomerByEmailAsync(Email);

            if(ExistCustomer is null)
            {
                return ApiResponse<Object>.FailureResponse("Customer not registered", new List<string> {$""}, 400);
            }

             if(await _repo.GetCustomerByEmailAsync(request.Email) != null)
            {
                return ApiResponse<Object>.FailureResponse("This Email Already exist. Please enter new Email", new List<string> {$""}, 400); 
            }

            ExistCustomer.Email = request.Email;
            await _repo.UpdateCustomerEmailAsync(ExistCustomer);
            return ApiResponse<Object>.SuccessResponse(null, "Email Updated Successfully",200);

        }

        public async Task<ApiResponse<RegisterResponseDTO?>> RegisterAgencyAsync (AgencyRegisterDTO request)
        {
             // check if customer already exist
            var agency = await _repo.GetAgencyByEmailAysnc(request.Email);

            if(agency != null)
            {
                return ApiResponse<RegisterResponseDTO?>.FailureResponse("Agency Already Exist", new List<string>{"Agency is already register"}, 400);
            }

            // this is new customer
            var NewAgency = new Agency();

            NewAgency.Name = request.Name;

            NewAgency.Email = request.Email.ToLower();

            NewAgency.Phone = request.Phone;

            var passwardhash = new PasswordHasher<Agency>().HashPassword(NewAgency, request.Password);

            NewAgency.PasswordHash = passwardhash;

            NewAgency.ContactPersonName = request.ContactPersonName;
            var saved = await _repo.RegisterAgencyAsync(NewAgency);

            var response = new RegisterResponseDTO{
                Name = saved.Name,
                Email = saved.Email,
                Phone = saved.Phone
            };

            return ApiResponse<RegisterResponseDTO?>.SuccessResponse(response, "Agency Register Successfully", 200);  
        }

        public async Task<ApiResponse<LoginResponseDTO?>> LoginAgencyAsync (LoginRequestDTO request)
        {
            // check is agency is not registered
            var agency = await _repo.GetAgencyByEmailAysnc(request.Email);

            if(agency is null)
            {
                return ApiResponse<LoginResponseDTO?>.FailureResponse("Agency not registered", new List<string>{$"{agency}"}, 400);
            }

            // check hash password
            var result = new PasswordHasher<Agency>().VerifyHashedPassword(agency, agency.PasswordHash, request.Password);

            if(result ==  PasswordVerificationResult.Failed)
            {
                return ApiResponse<LoginResponseDTO?>.FailureResponse("Invalid Credential", new List<string>{$"{agency}"}, 401);
            }

            var token = _tokenhelper.GenerateToken(agency.Name, agency.Email, agency.AgencyId, "Agency");

            var response = new LoginResponseDTO
            {
                Token = token
            };

            return ApiResponse<LoginResponseDTO?>.SuccessResponse(response, "Agency Login Successfully");
        }

        public async Task<ApiResponse<Object>> ForgetAgencyPasswordAsync (string Email, UpdatePasswordDTO request)
        {
            var existAgency = await _repo.GetAgencyByEmailAysnc(Email);

            if(existAgency is null)
            {
               return ApiResponse<Object>.FailureResponse("Agency not registered", new List<string> {$"{existAgency}"}, 400); 
            }

            var passwardhash = new PasswordHasher<Agency>().HashPassword(existAgency, request.Password);
            existAgency.PasswordHash = passwardhash;

            await _repo.UpdateAgencyPasswordAsync(existAgency);
            return ApiResponse<Object>.SuccessResponse(null, "Password Updated Successfully", 200);
        }

        public async Task<ApiResponse<Object>> UpdateAgencyEmailAsync (string Email, UpdateEmailDTO request)
        {
            var existAgency = await _repo.GetAgencyByEmailAysnc(Email);

            if(existAgency is null)
            {
                return ApiResponse<Object>.FailureResponse("Agency not registered", new List<string> {$""}, 400); 
            }

            if(await _repo.GetAgencyByEmailAysnc(request.Email) != null)
            {
                return ApiResponse<Object>.FailureResponse("This Email Already exist. Please enter new Email", new List<string> {$""}, 400); 
            }

            existAgency.Email = request.Email;
            await _repo.UpadateAgencyEmailAsync(existAgency);
            return ApiResponse<Object>.SuccessResponse(null, "Email Updated Successfully", 200);
        }
    }
}