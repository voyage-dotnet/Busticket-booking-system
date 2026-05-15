using BusTicketSystem.Web.DTOs;
using FluentValidation;

namespace BusTicketSystem.Web.Validator;

public class PaymentRequestValidator : AbstractValidator<PaymentRequestDTO>
{
    public static new List<string> Validate(PaymentRequestDTO dto)
    {
        var errors = new List<string>();

        if (dto.BookingId <= 0)
            errors.Add("BookingId must be a valid positive number.");

        if (dto.Amount <= 0)
            errors.Add("Amount must be greater than 0.");

        var validMethods = new[] { "UPI", "CARD" };
        if (string.IsNullOrWhiteSpace(dto.PaymentMethod) ||
            !validMethods.Contains(dto.PaymentMethod.ToUpper()))
            errors.Add("PaymentMethod must be 'UPI' or 'CARD'.");

        if (dto.PaymentMethod?.ToUpper() == "CARD")
        {
            if (string.IsNullOrWhiteSpace(dto.CardNumber) || dto.CardNumber.Length != 16)
                errors.Add("Card number must be exactly 16 digits.");

            if (string.IsNullOrWhiteSpace(dto.CardHolderName))
                errors.Add("Card holder name is required.");

            if (string.IsNullOrWhiteSpace(dto.Expiry))
                errors.Add("Card expiry is required.");

            if (string.IsNullOrWhiteSpace(dto.CVV) || (dto.CVV.Length < 3 || dto.CVV.Length > 4))
                errors.Add("CVV must be 3 or 4 digits.");
        }

        return errors;
    }
}