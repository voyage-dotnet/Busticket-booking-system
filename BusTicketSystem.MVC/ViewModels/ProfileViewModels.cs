using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BusTicketSystem.MVC.ViewModels
{
    public class ProfileViewModel
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("phone")]
        public string? Phone
        {
            get => PhoneNumber;
            set => PhoneNumber = value ?? string.Empty;
        }

        public string PhoneNumber { get; set; } = string.Empty;
        public int TotalTrips { get; set; }
        public DateTime MemberSince { get; set; }
    }

    public class UpdateProfileViewModel
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required, Phone]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
