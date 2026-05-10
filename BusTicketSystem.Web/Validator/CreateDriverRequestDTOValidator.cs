using BusTicketSystem.Web.DTOs;
using FluentValidation;

namespace BusTicketSystem.Web.Validator
{
    public class CreateDriverRequestDTOValidator : AbstractValidator<DriverCreateDTO>
    {
        public CreateDriverRequestDTOValidator()
        {
            RuleFor(x => x.OfficeId)
                .GreaterThan(0);

            RuleFor(x => x.AddressId)
                .GreaterThan(0);

            RuleFor(x => x.LicenseNumber)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Phone)
                .NotEmpty()
                .MaximumLength(20);
        }
    }
}