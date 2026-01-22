using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_back.Services;
using noMoreAzerty_dto.DTOs.Request;

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
        var userId = GetCurrentUserId();

        // Récupérer le coffre
        var vault = await _vaultRepository.GetByIdAsync(vaultId);
        if (vault == null)
            throw new NotFoundException("Vault not found");

        // Vérifier l'accès
        if (!await _vaultRepository.UserHasAccessToVaultAsync(vaultId, userId))
            throw new ForbiddenException("User does not have access to this vault");

        // Vérifier le mot de passe (BCrypt)
        var passwordToHash = $"{request.Password}{vault.PasswordSalt}";
        if (!BCrypt.Net.BCrypt.Verify(passwordToHash, vault.HashPassword))
            throw new ForbiddenException("Invalid password");

        _logger.LogInformation("Vault {VaultId} unlocked by user {UserId}", vaultId, userId);

        return Ok(true);
    }

    /// <summary>
    /// Stocke KEY_STORAGE en session
    /// </summary>
    [HttpPost("{vaultId:guid}/session/store-key")]
    public async Task<IActionResult> StoreKeyStorage(Guid vaultId, [FromBody] StoreKeyRequest request)
    {
        var userId = GetCurrentUserId();
        var userIp = GetUserIp();

        // Vérifier l'accès au coffre
        if (!await _vaultRepository.UserHasAccessToVaultAsync(vaultId, userId))
            throw new ForbiddenException("User does not have access to this vault");

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

    /// <summary>
    /// Récupère KEY_STORAGE depuis la session
    /// </summary>
    [HttpGet("{vaultId:guid}/session/key")]
    public async Task<IActionResult> GetKeyStorage(Guid vaultId)
    {
        var userId = GetCurrentUserId();
        var userIp = GetUserIp();

        // Vérifier l'accès
        if (!await _vaultRepository.UserHasAccessToVaultAsync(vaultId, userId))
            throw new ForbiddenException("User does not have access to this vault");

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
            throw new ForbiddenException("Session expired");
        }

        return Ok(new { keyStorage });
    }

    /// <summary>
    /// Supprime KEY_STORAGE (lock vault)
    /// </summary>
    [HttpDelete("{vaultId:guid}/session/key")]
    public IActionResult DeleteKeyStorage(Guid vaultId)
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

    private Guid GetCurrentUserId()
    {
        var oidClaim = User.FindFirst("oid")?.Value
                      ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;

        if (!Guid.TryParse(oidClaim, out var userId))
            throw new ForbiddenException("Invalid user ID");

        return userId;
    }

    private string GetUserIp()
    {
        return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}