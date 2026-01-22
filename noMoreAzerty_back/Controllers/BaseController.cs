using Microsoft.AspNetCore.Mvc;

namespace noMoreAzerty_back.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected Guid GetAuthenticatedUserId()
        {
            var oidClaim = User.FindFirst("oid")?.Value
                        ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (string.IsNullOrEmpty(oidClaim))
                throw new UnauthorizedAccessException("User is not authenticated");

            if (!Guid.TryParse(oidClaim, out var userId))
                throw new UnauthorizedAccessException("Invalid user identifier");

            return userId;
        }
    }
}
