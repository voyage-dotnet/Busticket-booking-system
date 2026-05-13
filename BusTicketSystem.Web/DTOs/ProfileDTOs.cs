namespace BusTicketSystem.Web.DTOs;
public class CustomerAddressRegisterDTO
{
    public string Address1 { get; set; } = null!;
    public string City { get; set; } = null!;
    public string State { get; set; } = null!;
    public string ZipCode { get; set; } = null!;
}
public class CustomerProfileDTO
{
    public int CustomerId { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public CustomerAddressRegisterDTO? Address { get; set; }
}
