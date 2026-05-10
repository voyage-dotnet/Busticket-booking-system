using BusTicketSystem.Web.DTOs;

namespace BusTicketSystem.Web.Validator
{
    public class SubmitReviewValidator
    {
        public static List<string> Validate(SubmitReviewDTO dto)
        {
            var errors = new List<string>();
            if (dto.TripId <= 0)
                errors.Add("TripId must be a valid positive number.");

            if (dto.Rating < 1 || dto.Rating > 5)
                errors.Add("Rating must be between 1 and 5.");

            if (dto.Comment != null && dto.Comment.Trim().Length == 0)
                errors.Add("Comment cannot be empty whitespace.");

            if (dto.Comment != null && dto.Comment.Length > 1000)
                errors.Add("Comment must not exceed 1000 characters.");

            return errors;
        }
    }
}
