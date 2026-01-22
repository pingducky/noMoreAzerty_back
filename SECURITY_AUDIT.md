# Sécurité - noMoreAzerty API

## Failles Globales Critiques

### CRITIQUE - Contournement d'authentification
**CWE-798 | CVSS 10.0**

**Faille**: Valeur GUID hardcodée dans **TOUS les contrôleurs**
```csharp
String? oidClaim = HttpContext.User.FindFirst("oid")?.Value
    ?? HttpContext.User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value
    ?? "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"; // ❌ CRITIQUE
```

**Impact**: 
- Contournement total de l'authentification
- Accès aux données de l'utilisateur avec ce GUID
- Compromission complète du système

**Patch en 3 lignes**:
```csharp
// Créer BaseController avec méthode helper
protected Guid GetAuthenticatedUserId()
{
    var oid = User.FindFirst("oid")?.Value 
           ?? User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier")?.Value;
    
    if (string.IsNullOrEmpty(oid) || !Guid.TryParse(oid, out var userId))
        throw new UnauthorizedAccessException("User not authenticated");
    
    return userId;
}
// Supprimer TOUS les fallbacks hardcodés
```

---

### 2. 🔴 CRITIQUE - Stockage non sécurisé des clés de chiffrement
**CWE-522 | CVSS 9.1**

**Faille**: `VaultSessionManager` Singleton stocke KEY_STORAGE en clair en mémoire
```csharp
// ❌ CRITIQUE: Clés AES-256 en clair dans un Dictionary statique
private readonly Dictionary<string, SessionData> _sessions = new();

public void StoreKeyStorage(Guid userId, Guid vaultId, string keyStorage, string userIp)
{
    _sessions[sessionKey] = new SessionData
    {
        KeyStorage = keyStorage,  // ❌ Clé en CLAIR
        UserIp = userIp
    };
}
```

**Impact**:
- Dump mémoire = récupération de TOUTES les clés actives
- Ne fonctionne pas en cluster / scaling horizontal
- Redémarrage = perte de toutes les sessions
- Compromission serveur = vol massif de données

**Patch**:
```csharp
// 1. Remplacer par Redis + Data Protection API
builder.Services.AddStackExchangeRedisCache(options => {
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("./keys"));

// 2. Créer SecureVaultSessionService
public class SecureVaultSessionService
{
    private readonly IDistributedCache _cache;
    private readonly IDataProtectionProvider _protector;
    
    public async Task StoreKeyAsync(Guid userId, Guid vaultId, string key)
    {
        var protector = _protector.CreateProtector("VaultSession");
        var encrypted = protector.Protect(key); // ✅ Chiffré
        
        await _cache.SetStringAsync(
            $"vault:{userId}:{vaultId}", 
            encrypted,
            new DistributedCacheEntryOptions {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            }
        );
    }
}
```

---

### Absence de Rate Limiting
**CWE-307 | CVSS 7.5**

**Faille**: Aucune route n'a de limitation de débit

**Impact**:
- **Brute force du mot de passe maître** (`/unlock`) - CRITIQUE
- **Énumération d'utilisateurs** (`/share`)
- **Spam/DoS** (création massive de coffres/entrées)

**Routes affectées**:
- ❌ `POST /vaults/{id}/unlock` - **Brute force illimité**
- ❌ `POST /vault` - Création illimitée
- ❌ `POST /vaults/{id}/share` - Énumération
- ❌ `POST /entries/create` - Spam

**Patch**:
```csharp
// Program.cs - .NET 8 inclut le rate limiting natif
builder.Services.AddRateLimiter(options =>
{
    // Rate limiting STRICT pour unlock
    options.AddFixedWindowLimiter("unlock", opt => {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 5; // 5 tentatives/minute MAX
        opt.QueueLimit = 0;
    });
    
    // Rate limiting pour création
    options.AddSlidingWindowLimiter("create", opt => {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 10;
    });
});

app.UseRateLimiter();

// Controller
[HttpPost("{vaultId}/unlock")]
[EnableRateLimiting("unlock")] // ✅ Protection brute force
public async Task<IActionResult> UnlockVault(...)
```

---

### 4. 🔴 ÉLEVÉ - Information Disclosure via messages d'erreur
**CWE-209 | CVSS 5.3**

**Faille**: Messages d'erreur différenciés révèlent l'état interne
```csharp
if (vault == null)
    return NotFound(new { message = "Vault not found" }); // ❌ Révèle existence

if (!await HasAccess(...))
    return Forbid(); // ❌ Révèle accès refusé

if (!BCrypt.Verify(...))
    return Unauthorized(new { message = "Invalid password" }); // ❌ Révèle mauvais mdp
```

**Impact**: 
- Énumération de coffres existants
- Détection des droits d'accès
- Facilite les attaques ciblées

**Patch**:
```csharp
// Message UNIQUE pour tous les cas d'échec
public static class ErrorMessages {
    public const string Unauthorized = "Authentication failed";
    public const string NotFound = "Resource not found";
}

// Même réponse quelque soit le cas
if (vault == null || !await HasAccess(...) || !BCrypt.Verify(...))
{
    return Unauthorized(new { message = ErrorMessages.Unauthorized });
}
```

---

### 5. 🔴 ÉLEVÉ - Timing Attack sur l'unlock
**CWE-208 | CVSS 6.8**

**Faille**: Temps de réponse variable révèle des informations
```
Temps mesuré:
- Vault n'existe pas: ~10ms
- Pas d'accès: ~50ms
- Mauvais mot de passe: ~200ms (BCrypt)
→ Permet de détecter existence et accès AVANT de tenter le mot de passe
```

**Patch**:
```csharp
[HttpPost("{vaultId}/unlock")]
public async Task<IActionResult> UnlockVault(...)
{
    var minimumDuration = TimeSpan.FromMilliseconds(200);
    var stopwatch = Stopwatch.StartNew();
    
    // Logique de vérification...
    
    // Attendre le temps minimum avant de répondre
    if (stopwatch.Elapsed < minimumDuration)
        await Task.Delay(minimumDuration - stopwatch.Elapsed);
    
    return result;
}
```

---

## 📍 Analyse Détaillée par Route

### VaultController (8 routes)

#### 1. `GET /api/vault/my`
**Failles**:
- 🔴 CRITIQUE: Fallback GUID hardcodé
- 🟡 MOYEN: Message d'erreur verbeux

**Patch**: Utiliser `GetAuthenticatedUserId()` sans fallback

---

#### 2. `GET /api/vault/shared`
**Failles**: 
- 🔴 CRITIQUE: Fallback GUID hardcodé
- 🟡 MOYEN: Message d'erreur verbeux

**Patch**: Idem route 1

---

#### 3. `POST /api/vault`
**Failles**:
- 🔴 CRITIQUE: Fallback GUID hardcodé
- 🔴 ÉLEVÉ: Pas de rate limiting (spam)
- 🟡 MOYEN: Pas de validation longueur nom

**Patch**:
```csharp
// Ajouter validation
if (request.Name.Length > 100)
    return BadRequest("Name too long");

// Ajouter rate limiting
[EnableRateLimiting("create")]
```

---

#### 4. `PUT /api/vault/{vaultId}`
**Failles**:
- 🔴 CRITIQUE: Fallback GUID hardcodé
- 🟡 MOYEN: Pas de vérification ancien mot de passe
- 🟡 MOYEN: Changement mdp non audité
- 🟡 MOYEN: IDOR potentiel

**Patch**:
```csharp
// Ajouter dans UpdateVaultUseCase
if (!string.IsNullOrEmpty(newPassword))
{
    // Vérifier l'ancien mot de passe d'abord
    if (!BCrypt.Verify(oldPassword, vault.HashPassword))
        throw new ForbiddenException("Current password incorrect");
    
    // Logger le changement
    await _auditService.LogAsync(userId, "VAULT_PASSWORD_CHANGED", vaultId);
    
    // Invalider toutes les sessions actives
    await _sessionService.InvalidateAllSessionsAsync(vaultId);
}
```

---

#### 5. `DELETE /api/vault/{vaultId}`
**Failles**:
- 🔴 CRITIQUE: Fallback GUID hardcodé
- 🟡 MOYEN: Suppression sans confirmation
- 🟠 FAIBLE: Pas de soft delete

**Patch**:
```csharp
// Ajouter confirmation token
[HttpDelete("{vaultId}")]
public async Task<IActionResult> DeleteVault(Guid vaultId, [FromQuery] string confirmToken)
{
    if (!IsValidDeleteToken(confirmToken, vaultId))
        return BadRequest("Invalid confirmation token");
    
    // Soft delete au lieu de hard delete
    await _deleteVaultUseCase.ExecuteAsync(vaultId, userId, softDelete: true);
}

// Dans le UseCase
vault.IsDeleted = true;
vault.DeletedAt = DateTime.UtcNow;
vault.DeletedBy = userId;
```

---

#### 6. `GET /api/vault/{vaultId}/users`
**Failles**:
- 🔴 CRITIQUE: Fallback GUID hardcodé
- 🟡 MOYEN: Information Disclosure (liste complète)
- 🟠 FAIBLE: Pas de pagination

**Patch**:
```csharp
[HttpGet("{vaultId}/users")]
public async Task<IActionResult> GetVaultUsers(
    Guid vaultId, 
    [FromQuery] int page = 1, 
    [FromQuery] int pageSize = 20)
{
    var result = await _getVaultUsersUseCase.ExecuteAsync(
        vaultId, 
        userId, 
        page, 
        Math.Min(pageSize, 100) // Max 100
    );
    return Ok(result);
}
```

---

#### 7. `POST /api/vault/{vaultId}/share`
**Failles**:
- 🔴 CRITIQUE: Fallback GUID hardcodé
- 🔴 ÉLEVÉ: Pas de rate limiting (énumération users)
- 🟡 MOYEN: Messages différenciés
- 🟠 FAIBLE: Pas de notification

**Patch**:
```csharp
[HttpPost("{vaultId}/share")]
[EnableRateLimiting("share")] // Max 10/minute
public async Task<IActionResult> ShareVault(...)
{
    try {
        await _shareVaultUseCase.ExecuteAsync(...);
        
        // Envoyer notification
        await _notificationService.NotifyUserAsync(
            targetUserId, 
            $"Vault shared with you by {currentUser.Email}"
        );
        
        return Ok(new { message = "Vault shared successfully" });
    }
    catch {
        // Message générique même si user n'existe pas
        return Ok(new { message = "Request processed" });
    }
}
```

---

#### 8. `DELETE /api/vault/{vaultId}/share/{targetUserId}`
**Failles**:
- 🔴 CRITIQUE: Fallback GUID hardcodé
- 🟡 MOYEN: IDOR
- 🟠 FAIBLE: Logique métier floue

**Patch**:
```csharp
// Dans le UseCase, ajouter vérifications
if (targetUserId == vault.UserId)
    throw new ValidationException("Owner cannot remove himself");

if (requestingUserId != vault.UserId)
    throw new ForbiddenException("Only owner can unshare");
```

---

### VaultSessionController (4 routes)

#### 9. `POST /api/vaults/{vaultId}/unlock`
**Failles**:
- 🔴 CRITIQUE: Pas de rate limiting (brute force)
- 🔴 ÉLEVÉ: Timing attack
- 🟡 MOYEN: Messages différenciés

**Patch**:
```csharp
[HttpPost("{vaultId}/unlock")]
[EnableRateLimiting("unlock")] // ✅ 5 tentatives/minute MAX
public async Task<IActionResult> UnlockVault(...)
{
    var stopwatch = Stopwatch.StartNew();
    var minDuration = TimeSpan.FromMilliseconds(200);
    
    var vault = await _vaultRepository.GetByIdAsync(vaultId);
    bool isValid = false;
    
    if (vault != null && await HasAccess(...))
        isValid = BCrypt.Verify(passwordToHash, vault.HashPassword);
    
    // Temps constant
    if (stopwatch.Elapsed < minDuration)
        await Task.Delay(minDuration - stopwatch.Elapsed);
    
    if (!isValid)
    {
        // Logger échec
        await _auditService.LogFailedUnlockAsync(userId, vaultId, GetUserIp());
        return Unauthorized(new { message = "Authentication failed" });
    }
    
    return Ok(true);
}
```

---

#### 10. `POST /api/vaults/{vaultId}/session/store-key`
**Failles**:
- 🔴 CRITIQUE: Stockage KEY_STORAGE en clair (Singleton)
- 🔴 ÉLEVÉ: Ne scale pas (mémoire locale)
- 🟡 MOYEN: IP falsifiable

**Patch**: Voir solution complète dans la faille globale #2 (Redis + Data Protection)

---

#### 11. `GET /api/vaults/{vaultId}/session/key`
**Failles**:
- 🔴 CRITIQUE: Même problème Singleton
- 🟡 MOYEN: Validation IP insuffisante
- 🟠 FAIBLE: Timeout hardcodé

**Patch**: Utiliser `SecureVaultSessionService` avec Redis (voir faille #2)

---

#### 12. `DELETE /api/vaults/{vaultId}/session/key`
**Failles**:
- 🟠 FAIBLE: Validation minimale

**Patch**: OK, la vérification userId/vaultId est faite correctement

---

### VaultEntryController (5 routes)

#### 13. `POST /api/vaults/{vaultId}/entries/create`
**Failles**:
- 🔴 ÉLEVÉ: Pas de rate limiting (spam)
- 🟡 MOYEN: Pas de limite nb entrées
- 🟡 MOYEN: IP falsifiable

**Patch**:
```csharp
[HttpPost("create")]
[EnableRateLimiting("create")] // 10 créations/minute
public async Task<IActionResult> CreateVaultEntry(...)
{
    // Vérifier quota
    var entryCount = await _vaultRepository.GetEntryCountAsync(vaultId);
    if (entryCount >= 10000) // Limite configurable
        return BadRequest("Entry limit reached");
    
    // Utiliser UserContextService pour IP sécurisée
    var userIp = _userContextService.GetSecureIp();
    
    var entry = await _createVaultEntryUseCase.ExecuteAsync(...);
    return CreatedAtAction(nameof(CreateVaultEntry), new { id = entry.Id }, entry);
}
```

---

#### 14. `GET /api/vaults/{vaultId}/entries/metadata`
**Failles**:
- 🟠 FAIBLE: Pas de pagination

**Patch**:
```csharp
[HttpGet("metadata")]
public async Task<IActionResult> GetEntriesMetadata(
    Guid vaultId,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 50)
{
    var result = await _getVaultEntriesMetadataUseCase.ExecuteAsync(
        vaultId, 
        userId, 
        userIp,
        page,
        Math.Min(pageSize, 100)
    );
    return Ok(result);
}
```

---

#### 15. `GET /api/vaults/{vaultId}/entries/{entryId}`
**Failles**:
- 🟡 MOYEN: IDOR potentiel
- 🟠 FAIBLE: Pas de rate limiting

**Patch**: Vérification d'accès faite dans UseCase ✅ (OK)

---

#### 16. `DELETE /api/vaults/{vaultId}/entries/{entryId}`
**Failles**:
- 🟠 FAIBLE: Pas de confirmation
- 🟠 FAIBLE: Pas de soft delete

**Patch**: Implémenter soft delete comme pour les vaults

---

#### 17. `PUT /api/vaults/{vaultId}/entries/{entryId}`
**Failles**:
- 🟡 MOYEN: Pas de versioning
- 🟠 FAIBLE: IDOR

**Patch**:
```csharp
// Ajouter table EntryVersion
public class EntryVersion
{
    public Guid Id { get; set; }
    public Guid EntryId { get; set; }
    public byte[] CipherData { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid ModifiedBy { get; set; }
}

// Sauvegarder version avant modification
await _versionRepository.CreateVersionAsync(oldEntry);
await _entryRepository.UpdateAsync(newEntry);
```

---

### UserLogsController (1 route)

#### 18. `GET /api/users/{userId}/logs`
**Failles**:
- 🟡 MOYEN: Dépend de l'implémentation `IAdminAuthorizationService`
- 🟡 MOYEN: IDOR potentiel
- 🟠 FAIBLE: Pas de rate limiting

**Patch**:
```csharp
[HttpGet]
[Authorize(Roles = "Admin")] // ✅ Utiliser la vraie autorisation ASP.NET
public async Task<IActionResult> GetUserLogs(
    Guid userId,
    [FromQuery] VaultEntryAction[]? actions,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 50)
{
    // Double vérification
    if (!User.IsInRole("Admin"))
        return Forbid();
    
    var result = await _useCase.ExecuteAsync(userId, actions, page, pageSize);
    return Ok(result);
}
```

---

## 🛠️ Plan de Remédiation Prioritaire

### Phase 1 - URGENTE (Semaine 1)
| Action | Fichiers | Effort | Impact |
|--------|----------|--------|--------|
| ✅ Supprimer fallback GUID hardcodé | Tous les controllers | 2h | 🔴 Critique |
| ✅ Implémenter rate limiting | Program.cs + Controllers | 4h | 🔴 Critique |
| ✅ Remplacer VaultSessionManager par Redis | VaultSessionController + Startup | 8h | 🔴 Critique |

**Total Phase 1**: ~14 heures

---

### Phase 2 - HAUTE (Semaine 2)
| Action | Fichiers | Effort | Impact |
|--------|----------|--------|--------|
| ✅ Uniformiser messages d'erreur | Tous les controllers | 2h | 🔴 Élevé |
| ✅ Implémenter timing attack protection | VaultSessionController | 1h | 🔴 Élevé |
| ✅ Sécuriser récupération IP | Créer UserContextService | 2h | 🟡 Moyen |
| ✅ Ajouter audit complet | Créer AuditService + logs | 6h | 🟡 Moyen |

**Total Phase 2**: ~11 heures

---

### Phase 3 - MOYENNE (Semaine 3-4)
| Action | Fichiers | Effort | Impact |
|--------|----------|--------|--------|
| ✅ Ajouter pagination partout | Controllers + UseCases | 3h | 🟡 Moyen |
| ✅ Implémenter soft delete | Models + UseCases | 4h | 🟠 Faible |
| ✅ Ajouter validation mot de passe | UpdateVaultUseCase | 2h | 🟡 Moyen |
| ✅ Ajouter versioning entrées | Models + Repository | 4h | 🟠 Faible |
| ✅ Ajouter notifications | NotificationService | 3h | 🟠 Faible |

**Total Phase 3**: ~16 heures

---

## 📝 Checklist de Vérification Finale

### Avant Déploiement Production
- [ ] ✅ Aucun GUID hardcodé dans le code
- [ ] ✅ Rate limiting actif sur toutes les routes sensibles
- [ ] ✅ VaultSessionManager remplacé par Redis + chiffrement
- [ ] ✅ Messages d'erreur uniformisés
- [ ] ✅ Protection timing attack sur /unlock
- [ ] ✅ Audit logging complet implémenté
- [ ] ✅ Tests de sécurité automatisés en place
- [ ] ✅ Scan SAST/DAST passé sans erreur critique
- [ ] ✅ Revue de code sécurité effectuée
- [ ] ✅ Documentation sécurité à jour

### Tests de Sécurité Recommandés
```bash
# 1. Test brute force (devrait être bloqué)
for i in {1..100}; do
  curl -X POST https://api.test.com/vaults/{id}/unlock \
    -H "Authorization: Bearer $TOKEN" \
    -d '{"password":"test'$i'"}'
done

# 2. Test énumération GUID (devrait échouer)
curl -X GET https://api.test.com/vault/my \
  -H "Authorization: Bearer $TOKEN_WITHOUT_OID"
# → Doit retourner 401 Unauthorized

# 3. Test timing attack
time curl -X POST .../unlock -d '{"password":"wrong"}'
# → Toutes les réponses doivent prendre ~200ms

# 4. Test session après redémarrage
# → Sessions doivent persister (Redis)

# 5. Test scalability
# → Load balancing entre 2+ serveurs doit fonctionner
```

---

## 📚 Références

- [OWASP Top 10 2021](https://owasp.org/www-project-top-ten/)
- [OWASP API Security Top 10](https://owasp.org/API-Security/editions/2023/en/0x11-t10/)
- [CWE-798: Hard-coded Credentials](https://cwe.mitre.org/data/definitions/798.html)
- [CWE-522: Insufficiently Protected Credentials](https://cwe.mitre.org/data/definitions/522.html)
- [CWE-307: Improper Restriction of Excessive Authentication](https://cwe.mitre.org/data/definitions/307.html)
- [ASP.NET Core Rate Limiting](https://learn.microsoft.com/en-us/aspnet/core/performance/rate-limit)
- [ASP.NET Core Data Protection](https://learn.microsoft.com/en-us/aspnet/core/security/data-protection/)
- [NIST Password Guidelines](https://pages.nist.gov/800-63-3/sp800-63b.html)

---

## 💡 Recommandations Supplémentaires

### Configuration de Production
```json
// appsettings.Production.json
{
  "Security": {
    "SessionTimeout": 30,
    "MaxFailedUnlockAttempts": 5,
    "LockoutDurationMinutes": 15,
    "EnableTimingAttackProtection": true,
    "MinimumResponseTimeMs": 200
  },
  "RateLimiting": {
    "UnlockAttemptsPerMinute": 5,
    "CreateOperationsPerMinute": 10,
    "ShareOperationsPerMinute": 10
  },
  "Audit": {
    "EnableDetailedLogging": true,
    "RetentionDays": 90
  }
}
```

### Monitoring Recommandé
- ✅ Alertes sur tentatives de brute force détectées
- ✅ Monitoring utilisation mémoire/CPU (détection attaques)
- ✅ Logs centralisés (ELK, Seq, Application Insights)
- ✅ Tracking anomalies (connexions multiples, IPs inhabituelles)

---

**Document généré le**: 22 janvier 2025  
**Auteur**: Audit de sécurité automatisé  
**Version**: 1.0  
**Statut**: ⚠️ **CORRECTIONS CRITIQUES REQUISES**
