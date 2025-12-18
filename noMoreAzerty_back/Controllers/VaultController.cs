using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noMoreAzerty_dto.DTOs.Request;
using noMoreAzerty_dto.DTOs.Response;
using noMoreAzerty_back.UseCases.Vaults;
using noMoreAzerty_back.UseCases.Entries;
using System.Security.Claims;

namespace noMoreAzerty_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VaultController : ControllerBase
    {
        private readonly GetUserVaultsUseCase _getUserVaultsUseCase;
        private readonly GetSharedVaultsUseCase _getSharedVaultsUseCase;
        private readonly CreateVaultUseCase _createVaultUseCase;
        private readonly GetVaultEntriesUseCase _getEntriesByVaultUseCase;
        private readonly ILogger<VaultController> _logger;

        public VaultController(
            GetUserVaultsUseCase getUserVaultsUseCase,
            GetSharedVaultsUseCase getSharedVaultsUseCase,
            CreateVaultUseCase createVaultUseCase,
            GetVaultEntriesUseCase getEntriesByVaultUseCase,
            ILogger<VaultController> logger)
        {
            _getUserVaultsUseCase = getUserVaultsUseCase;
            _getSharedVaultsUseCase = getSharedVaultsUseCase;
            _createVaultUseCase = createVaultUseCase;
            _getEntriesByVaultUseCase = getEntriesByVaultUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Récupération des coffres de l'utilisateur
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyVaults()
        {
            String? oidClaim = HttpContext.User.FindFirst("oid")?.Value
                           ?? HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                           ?? "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

            if (!Guid.TryParse(oidClaim, out var userId))
                return BadRequest("User ID claim is not a valid GUID.");

            _logger.LogInformation("GetMyVaults called with userId: {UserId}", userId);

            var vaults = await _getUserVaultsUseCase.ExecuteAsync(userId);

            var result = vaults.Select(v => new GetVaultResponse());

            return Ok(vaults);
        }


        /// <summary>
        /// Récupération des coffres partagés de l'utilisateur
        /// </summary>
        [HttpGet("shared")]
        public async Task<IActionResult> GetSharedVaults()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest("User ID claim is not a valid GUID.");

            var sharedVaults = await _getSharedVaultsUseCase.ExecuteAsync(userId);
            return Ok(sharedVaults);
        }

        /// <summary>
        /// Création d'un coffre
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateVault([FromBody] CreateVaultRequest request)
        {
            string oidClaim = User.FindFirst("oid")?.Value
                           ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                           ?? "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

            if (!Guid.TryParse(oidClaim, out var userId))
                return BadRequest("Invalid user id");

            // Création du coffre
            var vaultId = await _createVaultUseCase.ExecuteAsync(
                userId,
                request.Name,
                request.DerivedPassword,
                request.PasswordSalt
            );

            var vaultResponse = new
            {
                Id = vaultId,
                Name = request.Name,
                PasswordSalt = request.PasswordSalt,
                CreatedAt = DateTime.UtcNow
            };

            return CreatedAtAction(nameof(CreateVault), new { id = vaultId }, vaultResponse);
        }
    }
}
