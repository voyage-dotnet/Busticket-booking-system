using System;
using BusTicketSystem.Web.DTOs;
using FluentValidation;

namespace BusTicketSystem.Web.Validator;

public class CreateTripRequestValidator : AbstractValidator<CreateTripRequestDTO>
{
    public CreateTripRequestValidator()
    {
        RuleFor(x => x.RouteId).GreaterThan(0);
        RuleFor(x => x.BusId).GreaterThan(0);
        RuleFor(x => x.DriverId).GreaterThan(0);
        RuleFor(x => x.BoardingAddressId).GreaterThan(0);
        RuleFor(x => x.DepartureTime).GreaterThan(DateTime.Now);
        RuleFor(x => x.ArrivalTime).GreaterThan(x => x.DepartureTime);
        RuleFor(x => x.Fare).GreaterThan(0);
    }
}

public class CreateRouteRequestValidator : AbstractValidator<CreateRouteRequestDTO>
{
    public CreateRouteRequestValidator()
    {
        RuleFor(x => x.ToCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.FromCity).NotEmpty().MaximumLength(100);
        RuleFor(x => x.EstimatedDurationMinutes).GreaterThan(0);
    }
}
