using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.UseCases.Entries;
using noMoreAzerty_back.UseCases.Vaults;
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
        private readonly DeleteVaultEntryUseCase _deleteVaultEntryUseCase;
        private readonly UpdateVaultEntryUseCase _updateVaultEntryUseCase;
        private readonly GetVaultEntriesMetadataUseCase _getVaultEntriesMetadataUseCase;
        private readonly GetVaultEntryByIdUseCase _getVaultEntryByIdUseCase;

        public VaultEntryController(
            CreateVaultEntryUseCase createVaultEntryUseCase,
            DeleteVaultEntryUseCase deleteVaultEntryUseCase,
            UpdateVaultEntryUseCase updateVaultEntryUseCase,
            GetVaultEntriesMetadataUseCase getVaultEntriesMetadataUseCase,
            GetVaultEntryByIdUseCase getVaultEntryByIdUseCase)
        {
            _createVaultEntryUseCase = createVaultEntryUseCase;
            _deleteVaultEntryUseCase = deleteVaultEntryUseCase;
            _updateVaultEntryUseCase = updateVaultEntryUseCase;
            _getVaultEntriesMetadataUseCase = getVaultEntriesMetadataUseCase;
            _getVaultEntryByIdUseCase = getVaultEntryByIdUseCase;
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
                throw new ForbiddenException("Invalid user id");

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
        /// Récupération des métadonnées (titres) des entrées d'un coffre
        /// </summary>
        [HttpGet("metadata")]
        public async Task<IActionResult> GetEntriesMetadata(Guid vaultId)
        {
            String? oidClaim = User.FindFirst("oid")?.Value
                           ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (!Guid.TryParse(oidClaim, out var userId))
                throw new ForbiddenException("Invalid user id");

            String userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            var metadata = await _getVaultEntriesMetadataUseCase.ExecuteAsync(vaultId, userId, userIp);

            return Ok(metadata);
        }

        /// <summary>
        /// Récupération d'une entrée spécifique par son ID
        /// </summary>
        [HttpGet("{entryId}")]
        public async Task<IActionResult> GetEntryById(Guid vaultId, Guid entryId)
        {
            String? oidClaim = User.FindFirst("oid")?.Value
                           ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (!Guid.TryParse(oidClaim, out var userId))
                throw new ForbiddenException("Invalid user id");

            String userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            GetVaultEntriesResponse entry = await _getVaultEntryByIdUseCase.ExecuteAsync(vaultId, entryId, userId, userIp);

            return Ok(entry);
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
                throw new ForbiddenException("Invalid user id");

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
                throw new ForbiddenException("Invalid user id");

            String userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            GetVaultEntriesResponse updatedEntry = await _updateVaultEntryUseCase.ExecuteAsync(
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

            return Ok(updatedEntry);
        }
    }
}
