using Microsoft.AspNetCore.Mvc;
using noMoreAzerty_back.Models.Enums;
using noMoreAzerty_back.UseCases.History;
using noMoreAzerty_back.Interfaces.Services;

namespace noMoreAzerty_back.Controllers
{
    [ApiController]
    [Route("api/users/{userId:guid}/logs")]
    public class UserLogsController : ControllerBase
    {
        private readonly GetUserVaultEntryHistoryUseCase _useCase;
        private readonly IAdminAuthorizationService _adminAuthService;

        public UserLogsController(
            GetUserVaultEntryHistoryUseCase useCase,
            IAdminAuthorizationService adminAuthService)
        {
            _useCase = useCase;
            _adminAuthService = adminAuthService;
        }

        /// <summary>
        /// Récupération des journeaux utilisateurs pour un utilisateur ayant le rôle admin
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUserLogs(
            Guid userId,
            [FromQuery] VaultEntryAction[]? actions,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5)
        {
            if (!await _adminAuthService.IsAdminAuthorizedAsync(HttpContext))
                return Forbid("Admin role required");
            var result = await _useCase.ExecuteAsync(userId, actions, page, pageSize);
            return Ok(result);
        }
    }
}
