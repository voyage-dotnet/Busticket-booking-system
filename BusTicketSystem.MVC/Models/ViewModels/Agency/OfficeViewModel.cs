using System.ComponentModel.DataAnnotations;

namespace BusTicketSystem.MVC.Models.ViewModels.Agency
{
    /// <summary>
    /// Represents the full details of a single office returned from the API.
    /// </summary>
    public class OfficeViewModel
    {
        public int OfficeId { get; set; }
        public int AgencyId { get; set; }

        [Display(Name = "Office Email")]
        public string OfficeMail { get; set; } = string.Empty;

        [Display(Name = "Contact Person")]
        public string OfficeContactPersonName { get; set; } = string.Empty;

        [Display(Name = "Contact Number")]
        public string OfficeContactNumber { get; set; } = string.Empty;

        [Display(Name = "Address ID")]
        public int OfficeAddressId { get; set; }

        [Display(Name = "Full Address")]
        public string FullAddress { get; set; } = string.Empty;
    }

    /// <summary>
    /// Used for GET /api/agencies/me/offices — list offices for the logged-in agency.
    /// </summary>
    public class OfficeListViewModel
    {
        public List<OfficeViewModel> Offices { get; set; } = new();
    }

    /// <summary>
    /// Used for POST /api/agencies/me/offices — create new office form.
    /// </summary>
    public class CreateOfficeViewModel
    {
        [Required(ErrorMessage = "Office email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [Display(Name = "Office Email")]
        public string OfficeMail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact person name is required.")]
        [Display(Name = "Contact Person Name")]
        [StringLength(100)]
        public string OfficeContactPersonName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required.")]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [Display(Name = "Contact Number")]
        public string OfficeContactNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid Address ID.")]
        [Display(Name = "Address ID")]
        public int OfficeAddressId { get; set; }
    }

    /// <summary>
    /// Used for PUT /api/offices/{id} — update existing office form.
    /// </summary>
    public class UpdateOfficeViewModel
    {
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [Display(Name = "Office Email")]
        public string? OfficeMail { get; set; }

        [Display(Name = "Contact Person Name")]
        [StringLength(100)]
        public string? OfficeContactPersonName { get; set; }

        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [Display(Name = "Contact Number")]
        public string? OfficeContactNumber { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid Address ID.")]
        [Display(Name = "Address ID")]
        public int? OfficeAddressId { get; set; }
    }
}
