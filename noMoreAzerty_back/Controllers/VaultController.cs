using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using noMoreAzerty_back.UseCases.Vaults;
using noMoreAzerty_dto.DTOs.Request;
using System.Security.Claims;

namespace noMoreAzerty_back.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VaultController : BaseController
    {
        private readonly GetUserVaultsUseCase _getUserVaultsUseCase;
        private readonly GetSharedVaultsUseCase _getSharedVaultsUseCase;
        private readonly CreateVaultUseCase _createVaultUseCase;
        private readonly ILogger<VaultController> _logger;
        private readonly ShareVaultUseCase _shareVaultUseCase;
        private readonly UnshareVaultUseCase _unshareVaultUseCase;
        private readonly UpdateVaultUseCase _updateVaultUseCase;
        private readonly DeleteVaultUseCase _deleteVaultUseCase;
        private readonly GetVaultUsersUseCase _getVaultUsersUseCase;


        public VaultController(
            GetUserVaultsUseCase getUserVaultsUseCase,
            GetSharedVaultsUseCase getSharedVaultsUseCase,
            CreateVaultUseCase createVaultUseCase,
            ShareVaultUseCase shareVaultUseCase,
            UnshareVaultUseCase unshareVaultUseCase,
            UpdateVaultUseCase updateVaultNameUseCase,
            DeleteVaultUseCase deleteVaultUseCase,
            GetVaultUsersUseCase getVaultUsersUseCase,
            ILogger<VaultController> logger)
        {
            _getUserVaultsUseCase = getUserVaultsUseCase;
            _getSharedVaultsUseCase = getSharedVaultsUseCase;
            _createVaultUseCase = createVaultUseCase;
            _shareVaultUseCase = shareVaultUseCase;
            _unshareVaultUseCase = unshareVaultUseCase;
            _updateVaultUseCase = updateVaultNameUseCase;
            _deleteVaultUseCase = deleteVaultUseCase;
            _getVaultUsersUseCase = getVaultUsersUseCase;
            _logger = logger;
        }

        /// <summary>
        /// Récupération des coffres de l'utilisateur
        /// </summary>
        [HttpGet("my")]
        public async Task<IActionResult> GetMyVaults()
        {
            Guid userId = GetAuthenticatedUserId();

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
            Guid userId = GetAuthenticatedUserId();

            var sharedVaults = await _getSharedVaultsUseCase.ExecuteAsync(userId);

            return Ok(sharedVaults);
        }

        /// <summary>
        /// Création d'un coffre
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateVault([FromBody] CreateVaultRequest request)
        {
            Guid userId = GetAuthenticatedUserId();

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
        /// Modifier un coffre (nom et/ou mot de passe)
        /// </summary>
        [HttpPut("{vaultId}")]
        public async Task<IActionResult> UpdateVault(Guid vaultId, [FromBody] UpdateVaultRequest request)
        {
            Guid userId = GetAuthenticatedUserId();

            var updatedVault = await _updateVaultUseCase.ExecuteAsync(
                vaultId,
                userId,
                request.Name,
                request.NewDerivedPassword,
                request.PasswordSalt
            );

            return Ok(updatedVault);
        }

        /// <summary>
        /// Supprimer un coffre
        /// </summary>
        [HttpDelete("{vaultId}")]
        public async Task<IActionResult> DeleteVault(Guid vaultId)
        {
            Guid userId = GetAuthenticatedUserId();

            await _deleteVaultUseCase.ExecuteAsync(vaultId, userId);

            return NoContent();
        }

        /// <summary>
        /// Récupérer tous les utilisateurs ayant accès à un coffre
        /// </summary>
        [HttpGet("{vaultId}/users")]
        public async Task<IActionResult> GetVaultUsers(Guid vaultId)
        {
            Guid userId = GetAuthenticatedUserId();

            var result = await _getVaultUsersUseCase.ExecuteAsync(vaultId, userId);

            return Ok(result);
        }

        /// <summary>
        /// Partager un coffre avec un utilisateur
        /// </summary>
        [HttpPost("{vaultId}/share")]
        public async Task<IActionResult> ShareVault(Guid vaultId, [FromBody] ShareVaultRequest request)
        {
            Guid userId = GetAuthenticatedUserId();

            var result = await _shareVaultUseCase.ExecuteAsync(vaultId, userId, request.UserName);

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
            Guid userId = GetAuthenticatedUserId();

            var result = await _unshareVaultUseCase.ExecuteAsync(vaultId, userId, targetUserId);

            if (!result)
                return NotFound("Le coffre n'est pas partagé avec cet utilisateur.");

            return Ok(new { message = "Partage supprimé avec succès." });
        }
    }
}
