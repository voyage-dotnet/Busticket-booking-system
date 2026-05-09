using BusTicketSystem.Web.DTOs;
using FluentValidation;

namespace BusTicketSystem.Web.Validator
{
    public class CreateBusRequestDTOValidator : AbstractValidator<CreateBusRequestDTO>
    {
        public CreateBusRequestDTOValidator()
        {
            RuleFor(x => x.OfficeId)
                .GreaterThan(0);

            RuleFor(x => x.RegistrationNumber)
                .NotEmpty()
                .MaximumLength(50);

            RuleFor(x => x.Capacity)
                .GreaterThan(0)
                .LessThanOrEqualTo(100);

            RuleFor(x => x.Type)
                .NotEmpty()
                .MaximumLength(50);
        }
    }
}