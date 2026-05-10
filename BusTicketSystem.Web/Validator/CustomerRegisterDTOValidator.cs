using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusTicketSystem.Web.DTOs;
using FluentValidation;

namespace BusTicketSystem.Web.Validator
{
    public class CustomerRegisterDTOValidator : AbstractValidator<CustomerAddressRegisterDTO>
    {
        public CustomerRegisterDTOValidator()
        {
            RuleFor(a => a.Address1)
            .NotEmpty().WithMessage("Address must required")
            .MinimumLength(15)
            .MaximumLength(50);
            
            RuleFor(a => a.City)
            .NotEmpty().WithMessage("City must required")
            .Matches("^[a-zA-Z ]+$").WithMessage("Valid City Only");

            RuleFor(a => a.State)
            .NotEmpty().WithMessage("State must required")
            .Matches("^[a-zA-Z ]+$").WithMessage("Valid State Only");

            RuleFor(a => a.ZipCode)
            .NotEmpty().WithMessage("ZipCode is required")
            .Matches("^[0-9]+$").WithMessage("Valid ZipCode");
        }
    }
}