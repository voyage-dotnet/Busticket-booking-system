using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BusTicketSystem.MVC.Helper
{
    public class TokenHelper
    {
        public static bool IsTokenValid(string Token)
        {
            if(Token is null) return false;

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(Token);

            return jwt.ValidTo > DateTime.UtcNow;
        }

        public static string GetUserName(string Token)
        {
            if(Token is null) return "Register";

            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(Token);

            var Name = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

            if(Name is null)
            {
                return "Profile";
            }
            return Name;
            
        }

        public static string? GetUserRole(string? Token)
        {
            if (string.IsNullOrEmpty(Token)) return null;
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(Token);
            return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
        }

        public static string? GetUserEmail(string? Token)
        {
            if (string.IsNullOrEmpty(Token)) return null;
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(Token);
            return jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
        }
    }
}