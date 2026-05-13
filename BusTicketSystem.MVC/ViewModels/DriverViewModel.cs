using System.ComponentModel.DataAnnotations;

namespace BusTicketSystem.MVC.Models.ViewModels.Agency
{
    /// <summary>
    /// Represents driver data returned from the API for display in views.
    /// </summary>
    public class DriverViewModel
    {
        public int DriverId { get; set; }

        [Display(Name = "Driver Name")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Display(Name = "Phone")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Office ID")]
        public int OfficeId { get; set; }

        [Display(Name = "Address ID")]
        public int AddressId { get; set; }
    }

    /// <summary>
    /// Used for GET /api/offices/{officeId}/drivers — list all drivers for an office.
    /// </summary>
    public class DriverListViewModel
    {
        public int OfficeId { get; set; }
        public List<DriverViewModel> Drivers { get; set; } = new();
    }

    /// <summary>
    /// Used for POST /api/drivers — register a new driver form.
    /// </summary>
    public class CreateDriverViewModel
    {
        [Required(ErrorMessage = "Driver name is required.")]
        [StringLength(100)]
        [Display(Name = "Driver Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "License number is required.")]
        [StringLength(30, ErrorMessage = "License number cannot exceed 30 characters.")]
        [Display(Name = "License Number")]
        public string LicenseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [Display(Name = "Phone Number")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Office is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid office.")]
        [Display(Name = "Assigned Office")]
        public int OfficeId { get; set; }

        [Required(ErrorMessage = "Address ID is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid Address ID.")]
        [Display(Name = "Address ID")]
        public int AddressId { get; set; }

        // For dropdown list of offices
        public List<OfficeViewModel> AvailableOffices { get; set; } = new();
    }

    /// <summary>
    /// Used for PUT /api/drivers/{id} — update driver info form.
    /// </summary>
    public class UpdateDriverViewModel
    {
        [StringLength(100)]
        [Display(Name = "Driver Name")]
        public string? Name { get; set; }

        [StringLength(30)]
        [Display(Name = "License Number")]
        public string? LicenseNumber { get; set; }

        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [Display(Name = "Phone Number")]
        public string? Phone { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid office.")]
        [Display(Name = "Assigned Office")]
        public int? OfficeId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid Address ID.")]
        [Display(Name = "Address ID")]
        public int? AddressId { get; set; }
    }
}
