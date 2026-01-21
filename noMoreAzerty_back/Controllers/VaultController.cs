using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.UseCases.Entries;
using noMoreAzerty_back.UseCases.Vaults;
using noMoreAzerty_dto.DTOs.Request;
using noMoreAzerty_dto.DTOs.Response;
using System.Security.Claims;
using static noMoreAzerty_back.Repositories.VaultRepository;

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
        private readonly ILogger<VaultController> _logger;
        private readonly ShareVaultUseCase _shareVaultUseCase;
        private readonly UnshareVaultUseCase _unshareVaultUseCase;
        private readonly UpdateVaultNameUseCase _updateVaultNameUseCase;
        private readonly DeleteVaultUseCase _deleteVaultUseCase;

        public VaultController(
            GetUserVaultsUseCase getUserVaultsUseCase,
            GetSharedVaultsUseCase getSharedVaultsUseCase,
            CreateVaultUseCase createVaultUseCase,
            ShareVaultUseCase shareVaultUseCase,
            UnshareVaultUseCase unshareVaultUseCase,
            UpdateVaultNameUseCase updateVaultNameUseCase,
            DeleteVaultUseCase deleteVaultUseCase,
            ILogger<VaultController> logger)
        {
            _getUserVaultsUseCase = getUserVaultsUseCase;
            _getSharedVaultsUseCase = getSharedVaultsUseCase;
            _createVaultUseCase = createVaultUseCase;
            _shareVaultUseCase = shareVaultUseCase;
            _unshareVaultUseCase = unshareVaultUseCase;
            _updateVaultNameUseCase = updateVaultNameUseCase;
            _deleteVaultUseCase = deleteVaultUseCase;
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
            var vaultResponse = await _createVaultUseCase.ExecuteAsync(
                userId,
                request.Name,
                request.DerivedPassword,
                request.PasswordSalt
            );

            return CreatedAtAction(nameof(CreateVault), new { id = vaultResponse.Id }, vaultResponse);
        }

        /// <summary>
        /// Modifier le nom d'un coffre
        /// </summary>
        [HttpPut("{vaultId}")]
        public async Task<IActionResult> UpdateVaultName(Guid vaultId, [FromBody] UpdateVaultNameRequest request)
        {
            string oidClaim = User.FindFirst("oid")?.Value
                           ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                           ?? "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

            if (!Guid.TryParse(oidClaim, out var userId))
                return BadRequest("Invalid user id");

            var updatedVault = await _updateVaultNameUseCase.ExecuteAsync(vaultId, userId, request.Name);

            return Ok(updatedVault);
        }

        /// <summary>
        /// Supprimer un coffre
        /// </summary>
        [HttpDelete("{vaultId}")]
        public async Task<IActionResult> DeleteVault(Guid vaultId)
        {
            string oidClaim = User.FindFirst("oid")?.Value
                           ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                           ?? "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

            if (!Guid.TryParse(oidClaim, out var userId))
                return BadRequest("Invalid user id");

            await _deleteVaultUseCase.ExecuteAsync(vaultId, userId);

            return NoContent();
        }

        /// <summary>
        /// Partager un coffre avec un utilisateur
        /// </summary>
        [HttpPost("{vaultId}/share")]
        public async Task<IActionResult> ShareVault(Guid vaultId, [FromBody] ShareVaultRequest request)
        {
            string oidClaim = User.FindFirst("oid")?.Value
                           ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                           ?? "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

            if (!Guid.TryParse(oidClaim, out var userId))
                return BadRequest("Invalid user id");

                var result = await _shareVaultUseCase.ExecuteAsync(vaultId, userId, request.TargetUserId);

                if (!result)
                    return Conflict("Le coffre est déjà partagé avec cet utilisateur.");

                return Ok(new { message = "Coffre partagé avec succès." });
        }

        /// <summary>
        /// Supprimer le partage d'un coffre avec un utilisateur
        /// </summary>
        [HttpDelete("{vaultId}/share/{targetUserId}")]
        public async Task<IActionResult> UnshareVault(Guid vaultId, Guid targetUserId)
        {
            string oidClaim = User.FindFirst("oid")?.Value
                           ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
                           ?? "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa";

            if (!Guid.TryParse(oidClaim, out var userId))
                return BadRequest("Invalid user id");

                var result = await _unshareVaultUseCase.ExecuteAsync(vaultId, userId, targetUserId);

                if (!result)
                    return NotFound("Le coffre n'est pas partagé avec cet utilisateur.");

                return Ok(new { message = "Partage supprimé avec succès." });
        }

        public class UpdateVaultNameRequest
        {
            public string Name { get; set; } = null!;
        }
    }
}
