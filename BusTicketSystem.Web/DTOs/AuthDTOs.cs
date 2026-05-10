namespace BusTicketSystem.Web.DTOs;

public class LoginRequestDTO
{
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class LoginResponseDTO
{
    public string? Token { get; set; }
}

public class RegisterRequestDTO
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegisterResponseDTO
{
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
}
