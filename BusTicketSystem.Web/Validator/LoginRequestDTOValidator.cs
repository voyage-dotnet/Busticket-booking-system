using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusTicketSystem.Web.DTOs;
using FluentValidation;

namespace BusTicketSystem.Web.Validator
{
    public class LoginRequestDTOValidator : AbstractValidator<LoginRequestDTO>
    {
        public LoginRequestDTOValidator()
        {
            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is Required")
            .EmailAddress().WithMessage("Enter a valid Email");
    
        }
    }
}