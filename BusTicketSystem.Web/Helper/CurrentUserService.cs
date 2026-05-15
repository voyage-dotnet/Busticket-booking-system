using System.Security.Claims;

namespace BusTicketSystem.Web.Helper
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetAgencyId()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null)
                throw new UnauthorizedAccessException("User context not found.");

            var role = user.FindFirst(ClaimTypes.Role)?.Value
                       ?? user.FindFirst("role")?.Value;

            var entityType = user.FindFirst("entity_type")?.Value;

            if (role != "Agency" && entityType != "Agency")
                throw new UnauthorizedAccessException("Only agency users can access this resource.");

            var agencyIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                ?? user.FindFirst("sub")?.Value
                                ?? user.FindFirst("agencyId")?.Value
                                ?? user.FindFirst("AgencyId")?.Value;

            if (string.IsNullOrWhiteSpace(agencyIdClaim))
                throw new UnauthorizedAccessException("Agency ID not found in token.");

            return int.Parse(agencyIdClaim);
        }
    }
}
