using Microsoft.AspNetCore.Http;
using noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Interfaces.Services;
namespace noMoreAzerty_back.Service
{
    public class AdminAuthorizationService : IAdminAuthorizationService
    {

        private readonly IUserRepository _userRepository;

        public AdminAuthorizationService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<bool> IsAdminAuthorizedAsync(HttpContext context)
        {
            var tokenUserIdString = context.User.FindFirst("oid")?.Value
                ?? context.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (!Guid.TryParse(tokenUserIdString, out var tokenUserId))
                return false;

            return await _userRepository.IsUserAdminAsync(tokenUserId);
        }

        public async Task<bool> IsAdminAuthorizedAsync(Guid userId)
        {
            return await _userRepository.IsUserAdminAsync(userId);
        }
    }
}