using BusTicketSystem.Web.DTOs;
using FluentValidation;

namespace BusTicketSystem.Web.Validator;

public class BookingValidator : AbstractValidator<BookingRequestDto>
{
    public BookingValidator()
    {
        RuleFor(x => x.TripId)
            .GreaterThan(0)
            .WithMessage("TripId must be a positive number");

        RuleFor(x => x.SeatNumber)
            .GreaterThan(0)
            .WithMessage("SeatNumber must be a positive number");

        RuleFor(x => x.CustomerId)
            .GreaterThan(0)
            .WithMessage("CustomerId must be a positive number");
    }
}