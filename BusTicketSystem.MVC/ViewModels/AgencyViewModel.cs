using System.ComponentModel.DataAnnotations;

namespace BusTicketSystem.MVC.Models.ViewModels.Agency
{
    /// <summary>
    /// Represents a single agency's public-facing info (used in lists and detail pages).
    /// </summary>
    public class AgencyViewModel
    {
        public int AgencyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPersonName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    /// <summary>
    /// Used for GET /api/agencies — list all public agencies.
    /// </summary>
    public class AgencyListViewModel
    {
        public List<AgencyViewModel> Agencies { get; set; } = new();
    }

    /// <summary>
    /// Used for PUT /api/agencies/me — update own agency profile form.
    /// </summary>
    public class UpdateAgencyViewModel
    {
        [Required(ErrorMessage = "Agency name is required.")]
        [Display(Name = "Agency Name")]
        [StringLength(150, ErrorMessage = "Name cannot exceed 150 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact person name is required.")]
        [Display(Name = "Contact Person")]
        [StringLength(100)]
        public string ContactPersonName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;
    }
}
