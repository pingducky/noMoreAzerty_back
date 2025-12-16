using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noMoreAzerty_back.UseCases.Vaults;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using noMoreAzerty_dto.Dtos.Request;


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
        private readonly CreateVaultUseCase _createVaultUseCase;

        public VaultController(
            GetUserVaultsUseCase getUserVaultsUseCase,
            GetSharedVaultsUseCase getSharedVaultsUseCase,
            CreateVaultUseCase createVaultUseCase,
            ILogger<VaultController> logger)
        {
            _getUserVaultsUseCase = getUserVaultsUseCase;
            _getSharedVaultsUseCase = getSharedVaultsUseCase;
            _createVaultUseCase = createVaultUseCase;
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


        [HttpPost]
        public async Task<IActionResult> CreateVault([FromBody] CreateVaultRequestDto request)
        {
            var oidClaim = User.FindFirst("oid")?.Value
                           ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                           ?? "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

            if (!Guid.TryParse(oidClaim, out var userId))
                return BadRequest("Invalid user id");

            var vaultId = await _createVaultUseCase.ExecuteAsync(
                userId,
                request.Name,
                request.DerivedPassword,
                request.PasswordSalt
            );

            return CreatedAtAction(nameof(CreateVault), new { id = vaultId }, null);
        }
    }
}

public class CreateVaultRequestDto // Todo : placer dans lib partagé /!\
{
    public string Name { get; set; } = null!;

    /// <summary>
    /// Mot de passe dérivé côté client (ex: via PBKDF2/Argon2)
    /// </summary>
    public string DerivedPassword { get; set; } = null!;

    /// <summary>
    /// Sel généré côté client
    /// </summary>
    public string PasswordSalt { get; set; } = null!;
}