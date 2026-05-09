using BusTicketSystem.Web.DTOs;
using FluentValidation;

namespace BusTicketSystem.Web.Validator
{
    public class UpdateAgencyRequestDTOValidator : AbstractValidator<UpdateAgencyRequestDTO>
    {
        public UpdateAgencyRequestDTOValidator()
        {
            RuleFor(x => x)
                .Must(x =>
                    x.Name != null ||
                    x.ContactPersonName != null ||
                    x.Email != null ||
                    x.Phone != null)
                .WithMessage("At least one field is required for update.");

            When(x => x.Name != null, () =>
            {
                RuleFor(x => x.Name)
                    .NotEmpty()
                    .MaximumLength(100);
            });

            When(x => x.ContactPersonName != null, () =>
            {
                RuleFor(x => x.ContactPersonName)
                    .NotEmpty()
                    .MaximumLength(100);
            });

            When(x => x.Email != null, () =>
            {
                RuleFor(x => x.Email)
                    .NotEmpty()
                    .EmailAddress()
                    .MaximumLength(150);
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