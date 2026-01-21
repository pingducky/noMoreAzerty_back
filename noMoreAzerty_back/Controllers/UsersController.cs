using Microsoft.AspNetCore.Mvc;
using noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Interfaces.Services;
using noMoreAzerty_back.UseCases.Users;

namespace noMoreAzerty_back.Controllers
{
    [ApiController]
    [Route("api/users")] // Todo: filtrer par role admin
    public class UsersController : ControllerBase
    {
        private readonly GetUsersUseCase _getUsersUseCase;
        private readonly IAdminAuthorizationService _adminAuthService;

        public UsersController(
            GetUsersUseCase getUsersUseCase,
            IAdminAuthorizationService adminAuthService)
        {
            _getUsersUseCase = getUsersUseCase;
            _adminAuthService = adminAuthService;
        }

        /// <summary>
        /// Récupère TOUS les utilisateurs
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            if (!await _adminAuthService.IsAdminAuthorizedAsync(HttpContext))
                return Forbid("Admin role required");

            var result = await _getUsersUseCase.ExecuteAsync();
            return Ok(result);
        }
    }
}