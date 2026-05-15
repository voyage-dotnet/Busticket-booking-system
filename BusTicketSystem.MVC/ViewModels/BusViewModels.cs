using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusTicketSystem.MVC.ViewModels
{
    public class BusViewModel
    {
        public int BusId { get; set; }
        public string RegistrationNumber { get; set; } = string.Empty;

        [JsonPropertyName("type")]
        public string Type
        {
            get => BusType;
            set => BusType = value ?? string.Empty;
        }

        public string BusType { get; set; } = string.Empty;

        [JsonPropertyName("capacity")]
        public int Capacity
        {
            get => TotalCapacity;
            set => TotalCapacity = value;
        }

        public int TotalCapacity { get; set; }
        
        public string Status { get; set; } = string.Empty;
    }

    public class CreateBusViewModel
    {
        [Required]
        [RegularExpression(@"^[A-Z0-9\s-]+$", ErrorMessage = "Invalid registration format")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [JsonPropertyName("type")]
        public string BusType { get; set; } = string.Empty;

        [Required, Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100")]
        [JsonPropertyName("capacity")]
        public int TotalCapacity { get; set; }

        public int OfficeId { get; set; }
    }
}
