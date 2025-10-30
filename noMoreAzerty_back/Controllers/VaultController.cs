using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noMoreAzerty_back.UseCases.Vaults;
using System.Security.Claims;
using Microsoft.Extensions.Logging;


namespace noMoreAzerty_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VaultController : ControllerBase
    {
        private readonly GetUserVaultsUseCase _getUserVaultsUseCase;
        private readonly GetSharedVaultsUseCase _getSharedVaultsUseCase;
        private readonly ILogger<VaultController> _logger;


        public VaultController(
            GetUserVaultsUseCase getUserVaultsUseCase,
            GetSharedVaultsUseCase getSharedVaultsUseCase,
            ILogger<VaultController> logger) // Inject logger
        {
            _getUserVaultsUseCase = getUserVaultsUseCase;
            _getSharedVaultsUseCase = getSharedVaultsUseCase;
            _logger = logger;
        }


        /// <summary>
        /// Coffres créés par l’utilisateur
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyVaults()
        {
            var oidClaim = HttpContext.User.FindFirst("oid")?.Value
                           ?? HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;


            if (string.IsNullOrWhiteSpace(oidClaim))
            {
                // Fallback pour tests locaux
                oidClaim = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
            }

            if (!Guid.TryParse(oidClaim, out var userId))
                return BadRequest("User ID claim is not a valid GUID.");

            _logger.LogInformation("GetMyVaults called with userId: {UserId}", userId);

            var vaults = await _getUserVaultsUseCase.ExecuteAsync(userId);

            return Ok(vaults);
        }

        /// <summary>
        /// Coffres partagés avec l’utilisateur
        /// </summary>
        [HttpGet("shared")]
        public async Task<IActionResult> GetSharedVaults()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userIdClaim))
            {
                userIdClaim = "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";
            }

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest("User ID claim is not a valid GUID.");

            var sharedVaults = await _getSharedVaultsUseCase.ExecuteAsync(userId);
            return Ok(sharedVaults);
        }
    }
}
