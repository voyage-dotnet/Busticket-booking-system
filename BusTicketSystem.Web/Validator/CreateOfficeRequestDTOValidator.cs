using BusTicketSystem.Web.DTOs;
using FluentValidation;

namespace BusTicketSystem.Web.Validator
{
    public class CreateOfficeRequestDTOValidator : AbstractValidator<OfficeCreateDTO>
    {
        public CreateOfficeRequestDTOValidator()
        {
            RuleFor(x => x.OfficeMail)
                .NotEmpty()
                .EmailAddress()
                .MaximumLength(150);

            RuleFor(x => x.OfficeContactPersonName)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.OfficeContactNumber)
                .NotEmpty()
                .MaximumLength(20);

            RuleFor(x => x.OfficeAddressId)
                .GreaterThan(0);
        }
    }
}