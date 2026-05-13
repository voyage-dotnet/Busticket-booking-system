using System.ComponentModel.DataAnnotations;

namespace BusTicketSystem.MVC.ViewModels
{
    public class CustomerAddressViewModel
    {
        [Required(ErrorMessage = "Address is required")]
        public string Address1 { get; set; } = null!;

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = null!;

        [Required(ErrorMessage = "State is required")]
        public string State { get; set; } = null!;

        [Required(ErrorMessage = "Zip Code is required")]
        [RegularExpression(@"^\d{5,6}$", ErrorMessage = "Invalid Zip Code")]
        public string ZipCode { get; set; } = null!;
    }

    public class CustomerProfileViewModel
    {
        public int CustomerId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public CustomerAddressViewModel? Address { get; set; }
    }

    public class UpdateEmailViewModel
    {
        [Required(ErrorMessage = "Current email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "New email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string NewEmail { get; set; } = null!;
    }

    public class UpdatePasswordViewModel
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "New password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; } = null!;
    }
}
