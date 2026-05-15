using System.ComponentModel.DataAnnotations;

namespace BusTicketSystem.MVC.ViewModels
{
    public class OfficeViewModel
    {
        public int OfficeId { get; set; }
        public string OfficeMail { get; set; } = string.Empty;
        public string OfficeContactPersonName { get; set; } = string.Empty;
        public string OfficeContactNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int AgencyId { get; set; }
    }

    public class CreateOfficeViewModel
    {
        [Required, EmailAddress]
        public string OfficeMail { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Contact name can only contain letters and spaces")]
        public string OfficeContactPersonName { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number")]
        public string OfficeContactNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(200, MinimumLength = 10)]
        public string Address { get; set; } = string.Empty;
    }
}
