using System.ComponentModel.DataAnnotations;

namespace BusTicketSystem.MVC.ViewModels
{
    
    
    
    public class AgencyViewModel
    {
        public int AgencyId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ContactPersonName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    
    
    
    public class AgencyListViewModel
    {
        public List<AgencyViewModel> Agencies { get; set; } = new();
    }

    
    
    
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
