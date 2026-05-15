using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusTicketSystem.MVC.ViewModels
{
    public class DriverViewModel
    {
        public int DriverId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string LicenseNumber { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string Phone
        {
            get => PhoneNumber;
            set => PhoneNumber = value ?? string.Empty;
        }

        public string PhoneNumber { get; set; } = string.Empty;
        public double Rating { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateDriverViewModel
    {
        [Required]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^[A-Z0-9-]+$", ErrorMessage = "Invalid license format")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required, Phone]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit phone number")]
        [JsonPropertyName("phone")]
        public string PhoneNumber { get; set; } = string.Empty;

        public int OfficeId { get; set; }
        public int AddressId { get; set; }
    }
}
