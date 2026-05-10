using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace BusTicketSystem.Web.Helper;

public interface IUserHelper
{
    int GetUserId();
    string GetUserRole();
    bool IsCustomer();
    bool IsAgency();
}

public class UserHelper : IUserHelper
{
    private readonly IHttpContextAccessor _context;
    public UserHelper(IHttpContextAccessor context)
    {
        _context = context;
    }

    public int GetUserId()
    {
        var claim = _context.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        return claim != null ? int.Parse(claim.Value) : 0;
    }

    public string GetUserRole()
    {
        return _context.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty;
    }

    public bool IsAgency() => GetUserRole() == "Agency";
    public bool IsCustomer() => GetUserRole() == "Customer";

}
