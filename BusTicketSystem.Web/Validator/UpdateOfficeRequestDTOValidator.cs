using BusTicketSystem.Web.DTOs;
using FluentValidation;

namespace BusTicketSystem.Web.Validator
{
    public class UpdateOfficeRequestDTOValidator : AbstractValidator<OfficeUpdateDTO>
    {
        public UpdateOfficeRequestDTOValidator()
        {
            RuleFor(x => x)
                .Must(x =>
                    x.OfficeMail != null ||
                    x.OfficeContactPersonName != null ||
                    x.OfficeContactNumber != null ||
                    x.OfficeAddressId != null)
                .WithMessage("At least one field is required for update.");

            When(x => x.OfficeMail != null, () =>
            {
                RuleFor(x => x.OfficeMail)
                    .NotEmpty()
                    .EmailAddress()
                    .MaximumLength(150);
            });

            When(x => x.OfficeContactPersonName != null, () =>
            {
                RuleFor(x => x.OfficeContactPersonName)
                    .NotEmpty()
                    .MaximumLength(100);
            });

            When(x => x.OfficeContactNumber != null, () =>
            {
                RuleFor(x => x.OfficeContactNumber)
                    .NotEmpty()
                    .MaximumLength(20);
            });

            When(x => x.OfficeAddressId != null, () =>
            {
                RuleFor(x => x.OfficeAddressId)
                    .GreaterThan(0);
            });
        }
    }
}