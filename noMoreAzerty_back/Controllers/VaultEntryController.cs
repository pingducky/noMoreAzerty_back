using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noMoreAzerty_back.UseCases.Entries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace noMoreAzerty_back.Controllers
{
    [ApiController]
    [Route("api/vaults/{vaultId}/entries")]
    [Authorize]
    public class VaultEntryController : ControllerBase
    {
        private readonly CreateVaultEntryUseCase _createVaultEntryUseCase;
        private readonly GetVaultEntriesUseCase _getVaultEntriesUseCase;

        public VaultEntryController(
            CreateVaultEntryUseCase createVaultEntryUseCase,
            GetVaultEntriesUseCase getVaultEntriesUseCase)
        {
            _createVaultEntryUseCase = createVaultEntryUseCase;
            _getVaultEntriesUseCase = getVaultEntriesUseCase;
        }

        // ---------------------------------------------
        // POST: Créer une entrée dans un coffre
        // ---------------------------------------------
        [HttpPost("create")]
        public async Task<IActionResult> CreateVaultEntry(Guid vaultId, [FromBody] CreateVaultEntryRequestDto request)
        {
            var userIdClaim = User.FindFirst("oid")?.Value
                              ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (!Guid.TryParse(userIdClaim, out var userId))
                return BadRequest("Invalid user id");

            var userIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            try
            {
                var entryId = await _createVaultEntryUseCase.ExecuteAsync(
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

                return CreatedAtAction(nameof(CreateVaultEntry), new { id = entryId }, null);
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Vault not found");
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid("User is not owner");
            }
        }

        // ---------------------------------------------
        // POST: Récupérer les entrées d'un coffre
        // (On garde POST car on doit envoyer le mot de passe)
        // ---------------------------------------------
        [HttpPost("access")]
        public async Task<IActionResult> GetVaultEntries(Guid vaultId, [FromBody] VaultAccessRequestDto request)
        {
            var oidClaim = User.FindFirst("oid")?.Value
                           ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

            if (!Guid.TryParse(oidClaim, out var userId))
                return BadRequest("Invalid user id");

            try
            {
                var entries = await _getVaultEntriesUseCase.ExecuteAsync(vaultId, userId, request.Password);

                var result = entries.Select(e => new VaultEntryDto
                {
                    Id = e.Id,
                    CipherTitle = e.CipherTitle,
                    TitleIV = e.TitleIV,
                    TitleTag = e.TitleTag,
                    CipherUsername = e.CipherUsername,
                    UsernameIV = e.UsernameIV,
                    UsernameTag = e.UsernameTag,
                    CipherPassword = e.CipherPassword,
                    PasswordIV = e.PasswordIV,
                    PasswordTag = e.PasswordTag,
                    CipherUrl = e.CipherUrl,
                    UrlIV = e.UrlIV,
                    UrlTag = e.UrlTag,
                    CipherCommentary = e.CipherCommentary,
                    ComentaryIV = e.ComentaryIV,
                    ComentaryTag = e.ComentaryTag,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                }).ToList();

                return Ok(result);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
        }

        // ---------------------------------------------
        // DTOs
        // ---------------------------------------------
        public class CreateVaultEntryRequestDto
        {
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
            public string Password { get; set; } = null!;
        }
    }
}
