using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusTicketSystem.Web.DTOs;
using FluentValidation;
using Microsoft.Identity.Client.Extensions.Msal;

namespace BusTicketSystem.Web.Validator
{
    public class RegisterRequestDTOValidator : AbstractValidator<RegisterRequestDTO>
    {
        public RegisterRequestDTOValidator()
        {
            RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("First name is required")
            .MinimumLength(5);

            RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .WithMessage("Enter Valid Email");

            RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password must required")
            .MinimumLength(8).WithMessage("Password must be atleast 8 character")
            .Matches("[A-Z]").WithMessage("Password must contain uppercase letter")
            .Matches("[a-z]").WithMessage("Password must contain lowercase letter")
            .Matches("[0-9]").WithMessage("Password must contain digit")
            .Matches("[^a-zA-Z0-9]").WithMessage("Password must contain special character");

            RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Phone Number is required")
            .Matches(@"^[0-9]{10}$");
            
        }
    }
}