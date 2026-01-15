using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Models;
using noMoreAzerty_back.UseCases.Entries;
using noMoreAzerty_dto.DTOs.Request;
using noMoreAzerty_dto.DTOs.Response;

namespace noMoreAzerty_back.Controllers
{
    [ApiController]
    [Route("api/vaults/{vaultId}/entries")]
    [Authorize]
    public class VaultEntryController : ControllerBase
    {
        private readonly CreateVaultEntryUseCase _createVaultEntryUseCase;
        private readonly GetVaultEntriesUseCase _getVaultEntriesUseCase;
        private readonly DeleteVaultEntryUseCase _deleteVaultEntryUseCase;
        private readonly UpdateVaultEntryUseCase _updateVaultEntryUseCase;

        public VaultEntryController(
            CreateVaultEntryUseCase createVaultEntryUseCase,
            GetVaultEntriesUseCase getVaultEntriesUseCase,
            DeleteVaultEntryUseCase deleteVaultEntryUseCase,
            UpdateVaultEntryUseCase updateVaultEntryUseCase)
        {
            _createVaultEntryUseCase = createVaultEntryUseCase;
            _getVaultEntriesUseCase = getVaultEntriesUseCase;
            _deleteVaultEntryUseCase = deleteVaultEntryUseCase;
            _updateVaultEntryUseCase = updateVaultEntryUseCase;
        }


        /// <summary>
        /// Création d'une nouvelle entrée de coffre
        /// </summary>
        [HttpPost("create")]
        public async Task<IActionResult> CreateVaultEntry(Guid vaultId, [FromBody] CreateVaultEntryRequest request)
        {
            string? userIdClaim = User.FindFirst("oid")?.Value
                               ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                throw new ValidationException("Invalid user id");

            string? userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            GetVaultEntriesResponse vaultEntry = await _createVaultEntryUseCase.ExecuteAsync(
                userId,
                vaultId,
                userIp,
                request.CipherTitle,
                request.TitleIV,
                request.TitleTag,
                request.CipherUsername,
                request.UsernameIV,
                request.UsernameTag,
                request.CipherPassword,
                request.PasswordIV,
                request.PasswordTag,
                request.CipherUrl,
                request.UrlIV,
                request.UrlTag,
                request.CipherCommentary,
                request.ComentaryIV,
                request.ComentaryTag
            );

            return CreatedAtAction(nameof(CreateVaultEntry), new { id = vaultEntry.Id }, vaultEntry);
        }


        /// <summary>
        /// Récupération des entrés d'un coffre (encore chiffré)
        /// </summary>
        [HttpPost("access")]
        public async Task<IActionResult> GetVaultEntries(Guid vaultId, [FromBody] VaultAccessRequest request)
        {
            String? oidClaim = User.FindFirst("oid")?.Value
                           ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (!Guid.TryParse(oidClaim, out var userId))
                throw new ValidationException("Invalid user id");

            var entries = await _getVaultEntriesUseCase.ExecuteAsync(vaultId, userId, request.Password);

            return Ok(entries);
        }

        /// <summary>
        /// Suppression d'une entré d'un coffre
        /// </summary>
        [HttpDelete("{entryId}")]
        public async Task<IActionResult> DeleteEntry(Guid vaultId, Guid entryId)
        {
            String? oidClaim = User.FindFirst("oid")?.Value
                           ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (!Guid.TryParse(oidClaim, out var userId))
                throw new ValidationException("Invalid user id");

            String userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            await _deleteVaultEntryUseCase.ExecuteAsync(
                userId,
                vaultId,
                entryId,
                userIp
            );

            return NoContent();
        }


        /// <summary>
        /// Mise à jour d'une entré d'un coffre
        /// </summary>
        [HttpPut("{entryId}")]
        public async Task<IActionResult> UpdateEntry(
            Guid vaultId,
            Guid entryId,
            [FromBody] UpdateVaultEntryRequest request)
        {
            String? oidClaim = User.FindFirst("oid")?.Value
                            ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (!Guid.TryParse(oidClaim, out var userId))
                throw new ValidationException("Invalid user id");

            String userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            await _updateVaultEntryUseCase.ExecuteAsync(
                userId,
                vaultId,
                entryId,
                userIp,
                request.CipherTitle,
                request.TitleIV,
                request.TitleTag,
                request.CipherUsername,
                request.UsernameIV,
                request.UsernameTag,
                request.CipherPassword,
                request.PasswordIV,
                request.PasswordTag,
                request.CipherUrl,
                request.UrlIV,
                request.UrlTag,
                request.CipherCommentary,
                request.ComentaryIV,
                request.ComentaryTag
            );

            return NoContent();
        }
    }
}
