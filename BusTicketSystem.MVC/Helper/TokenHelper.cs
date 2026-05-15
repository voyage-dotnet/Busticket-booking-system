using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BusTicketSystem.MVC.Helper
{
    public static class TokenHelper
    {
        public static string? GetUserRole(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return null;

            var jwtToken = handler.ReadJwtToken(token);
            var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role");
            
            return roleClaim?.Value;
        }

        public static string? GetUserEmail(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;
            
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return null;

            var jwtToken = handler.ReadJwtToken(token);
            var emailClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email || c.Type == "email" || c.Type == "sub");
            
            return emailClaim?.Value;
        }

        public static string? GetUserName(string token)
        {
            if (string.IsNullOrEmpty(token)) return null;

            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return null;

            var jwtToken = handler.ReadJwtToken(token);
            var nameClaim = jwtToken.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Name || c.Type == "name" || c.Type == "unique_name");

            return nameClaim?.Value;
        }
    }
}
