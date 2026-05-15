using BusTicketSystem.Web.DTOs;
using FluentValidation;

namespace BusTicketSystem.Web.Validator
{
    public class UpdateDriverRequestDTOValidator : AbstractValidator<DriverUpdateDTO>
    {
        public UpdateDriverRequestDTOValidator()
        {
            RuleFor(x => x)
                .Must(x =>
                    x.AddressId != null ||
                    x.LicenseNumber != null ||
                    x.Name != null ||
                    x.Phone != null)
                .WithMessage("At least one field is required for update.");

            When(x => x.AddressId != null, () =>
            {
                RuleFor(x => x.AddressId)
                    .GreaterThan(0);
            });

            When(x => x.LicenseNumber != null, () =>
            {
                RuleFor(x => x.LicenseNumber)
                    .NotEmpty()
                    .MaximumLength(50);
            });

            When(x => x.Name != null, () =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .MaximumLength(100);
            });

            When(x => x.Phone != null, () =>
            {
                RuleFor(x => x.Phone)
                    .NotEmpty()
                    .MaximumLength(20);
            });
        }
    }
}