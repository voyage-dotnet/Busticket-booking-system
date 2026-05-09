using BusTicketSystem.Web.DTOs;
using FluentValidation;

namespace BusTicketSystem.Web.Validator
{
    public class UpdateBusRequestDTOValidator : AbstractValidator<UpdateBusRequestDTO>
    {
        public UpdateBusRequestDTOValidator()
        {
            RuleFor(x => x)
                .Must(x =>
                    x.RegistrationNumber != null ||
                    x.Capacity != null ||
                    x.Type != null)
                .WithMessage("At least one field is required for update.");

            When(x => x.RegistrationNumber != null, () =>
            {
                RuleFor(x => x.RegistrationNumber)
                    .NotEmpty()
                    .MaximumLength(50);
            });

            When(x => x.Capacity != null, () =>
            {
                RuleFor(x => x.Capacity)
                    .GreaterThan(0)
                    .LessThanOrEqualTo(100);
            });

            When(x => x.Type != null, () =>
            {
                RuleFor(x => x.Type)
                    .NotEmpty()
                    .MaximumLength(50);
            });
        }
    }
}