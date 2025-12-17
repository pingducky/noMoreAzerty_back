using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using noMoreAzerty_back.UseCases.Vaults;
using noMoreAzerty_back.UseCases.Entries;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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
        /// Coffres créés par l’utilisateur
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyVaults()
        {
            var oidClaim = HttpContext.User.FindFirst("oid")?.Value
                           ?? HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                           ?? "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                              ?? "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest("User ID claim is not a valid GUID.");

            var sharedVaults = await _getSharedVaultsUseCase.ExecuteAsync(userId);
            return Ok(sharedVaults);
        }

        /// <summary>
        /// Crée un nouveau coffre
        /// </summary>
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

    #region DTO temporaires

    public class CreateVaultRequestDto
    {
        public string Name { get; set; } = null!;

        /// <summary>
        /// Mot de passe dérivé côté client (PBKDF2/Argon2)
        /// </summary>
        public string DerivedPassword { get; set; } = null!;

        /// <summary>
        /// Sel généré côté client
        /// </summary>
        public string PasswordSalt { get; set; } = null!;
    }

    public class VaultEntryDto
    {
        public Guid Id { get; set; }
        public string? CipherTitle { get; set; }
        public string? TitleIV { get; set; }
        public string? TitleTag { get; set; }

        public string? CipherUsername { get; set; }
        public string? UsernameIV { get; set; }
        public string? UsernameTag { get; set; }

        public string? CipherPassword { get; set; }
        public string? PasswordIV { get; set; }
        public string? PasswordTag { get; set; }

        public string? CipherUrl { get; set; }
        public string? UrlIV { get; set; }
        public string? UrlTag { get; set; }

        public string? CipherCommentary { get; set; }
        public string? ComentaryIV { get; set; }
        public string? ComentaryTag { get; set; }

        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class VaultAccessRequestDto
    {
        /// <summary>
        /// Mot de passe en clair envoyé par le client
        /// </summary>
        public string Password { get; set; } = null!;
    }

    #endregion
}
