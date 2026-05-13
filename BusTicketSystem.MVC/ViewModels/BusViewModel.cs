using System.ComponentModel.DataAnnotations;

namespace BusTicketSystem.MVC.ViewModels
{
    
    
    
    public class BusViewModel
    {
        public int BusId { get; set; }

        [Display(Name = "Office ID")]
        public int OfficeId { get; set; }

        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Display(Name = "Capacity")]
        public int Capacity { get; set; }

        [Display(Name = "Bus Type")]
        public string Type { get; set; } = string.Empty;
    }

    
    
    
    public class BusListViewModel
    {
        public int OfficeId { get; set; }
        public List<BusViewModel> Buses { get; set; } = new();
    }

    
    
    
    public class CreateBusViewModel
    {
        [Required(ErrorMessage = "Office is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid office.")]
        [Display(Name = "Office")]
        public int OfficeId { get; set; }

        [Required(ErrorMessage = "Registration number is required.")]
        [StringLength(20, ErrorMessage = "Registration number cannot exceed 20 characters.")]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Capacity is required.")]
        [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100.")]
        [Display(Name = "Seat Capacity")]
        public int Capacity { get; set; }

        [Required(ErrorMessage = "Bus type is required.")]
        [StringLength(50)]
        [Display(Name = "Bus Type")]
        public string Type { get; set; } = string.Empty;

        
        public List<OfficeViewModel> AvailableOffices { get; set; } = new();
    }

    
    
    
    public class UpdateBusViewModel
    {
        [StringLength(20, ErrorMessage = "Registration number cannot exceed 20 characters.")]
        [Display(Name = "Registration Number")]
        public string? RegistrationNumber { get; set; }

        [Range(1, 100, ErrorMessage = "Capacity must be between 1 and 100.")]
        [Display(Name = "Seat Capacity")]
        public int? Capacity { get; set; }

        [StringLength(50)]
        [Display(Name = "Bus Type")]
        public string? Type { get; set; }
    }
}
