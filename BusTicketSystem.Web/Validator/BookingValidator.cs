using BusTicketSystem.Web.DTOs;
using FluentValidation;

namespace BusTicketSystem.Web.Validator;

public class BookingValidator : AbstractValidator<BookingRequestDTO>
{
    public static List<string> ManualValidate(BookingRequestDTO dto)
    {
        var errors = new List<string>();

        if (dto.TripId <= 0)
            errors.Add("TripId must be a valid positive number.");

        if (dto.SeatNumber <= 0)
            errors.Add("SeatNumber must be a valid positive number.");

        return errors;
    }
}