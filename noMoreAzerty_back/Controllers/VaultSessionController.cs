using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_back.Services;

namespace noMoreAzerty_back.Controllers;

[ApiController]
[Route("api/vaults")]
[Authorize]
public class VaultSessionController : ControllerBase
{
    private readonly IVaultRepository _vaultRepository;
    private readonly ILogger<VaultSessionController> _logger;
    private static readonly TimeSpan SessionTimeout = TimeSpan.FromMinutes(30);

    public VaultSessionController(
        IVaultRepository vaultRepository,
        ILogger<VaultSessionController> logger)
    {
        _vaultRepository = vaultRepository;
        _logger = logger;
    }

    /// <summary>
    /// Vérifie le mot de passe du coffre
    /// </summary>
    [HttpPost("{vaultId:guid}/unlock")]
    public async Task<IActionResult> UnlockVault(Guid vaultId, [FromBody] UnlockRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();

            // Récupérer le coffre
            var vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                return NotFound(new { message = "Vault not found" });

            // Vérifier l'accès
            if (!await _vaultRepository.UserHasAccessToVaultAsync(vaultId, userId))
                return Forbid();

            // Vérifier le mot de passe (BCrypt)
            var passwordToHash = $"{request.Password}{vault.PasswordSalt}";
            if (!BCrypt.Net.BCrypt.Verify(passwordToHash, vault.HashPassword))
                return Unauthorized(new { message = "Invalid password" });

            _logger.LogInformation("Vault {VaultId} unlocked by user {UserId}", vaultId, userId);

            return Ok(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error unlocking vault {VaultId}", vaultId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Stocke KEY_STORAGE en session
    /// </summary>
    [HttpPost("{vaultId:guid}/session/store-key")]
    public async Task<IActionResult> StoreKeyStorage(Guid vaultId, [FromBody] StoreKeyRequest request)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userIp = GetUserIp();

            // Vérifier l'accès au coffre
            if (!await _vaultRepository.UserHasAccessToVaultAsync(vaultId, userId))
                return Forbid();

            // Stocker KEY_STORAGE dans VaultSessionManager
            var sessionManager = VaultSessionManager.Instance;
            sessionManager.StoreKeyStorage(userId, vaultId, request.KeyStorage, userIp);

            _logger.LogInformation(
                "KEY_STORAGE stored for vault {VaultId}, user {UserId}",
                vaultId,
                userId
            );

            return Ok(new { message = "Key storage saved" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error storing key for vault {VaultId}", vaultId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Récupère KEY_STORAGE depuis la session
    /// </summary>
    [HttpGet("{vaultId:guid}/session/key")]
    public async Task<IActionResult> GetKeyStorage(Guid vaultId)
    {
        try
        {
            var userId = GetCurrentUserId();
            var userIp = GetUserIp();

            // Vérifier l'accès
            if (!await _vaultRepository.UserHasAccessToVaultAsync(vaultId, userId))
                return Forbid();

            // Récupérer KEY_STORAGE depuis VaultSessionManager
            var sessionManager = VaultSessionManager.Instance;
            var keyStorage = sessionManager.GetKeyStorage(userId, vaultId, userIp, SessionTimeout);

            if (string.IsNullOrEmpty(keyStorage))
            {
                _logger.LogWarning(
                    "Session expired or invalid for vault {VaultId}, user {UserId}",
                    vaultId,
                    userId
                );
                return Unauthorized(new { message = "Session expired" });
            }

            return Ok(new { keyStorage });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving key for vault {VaultId}", vaultId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    /// <summary>
    /// Supprime KEY_STORAGE (lock vault)
    /// </summary>
    [HttpDelete("{vaultId:guid}/session/key")]
    public IActionResult DeleteKeyStorage(Guid vaultId)
    {
        try
        {
            var userId = GetCurrentUserId();

            // Supprimer la session
            var sessionManager = VaultSessionManager.Instance;
            sessionManager.RemoveSession(userId, vaultId);

            _logger.LogInformation(
                "Session removed for vault {VaultId}, user {UserId}",
                vaultId,
                userId
            );

            return Ok(new { message = "Session cleared" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error clearing session for vault {VaultId}", vaultId);
            return StatusCode(500, new { message = "Internal server error" });
        }
    }

    private Guid GetCurrentUserId()
    {
        var oidClaim = User.FindFirst("oid")?.Value
                      ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

        if (!Guid.TryParse(oidClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user ID");

        return userId;
    }

    private string GetUserIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}

// DTOs
public record UnlockRequest(string Password);
public record StoreKeyRequest(string KeyStorage);