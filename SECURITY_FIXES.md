# Failles de Sécurité par Route

## VaultController

### 3. `POST /api/vault`
**Faille**: 
- Pas de rate limiting (spam création)
- Pas de limite longueur nom

**Patch**: 
- TODO - Ajouter `[EnableRateLimiting("create")]`
- TODO - Valider `request.Name.Length <= 100`

---

### 4. `PUT /api/vault/{vaultId}`
**Faille**:
- Changement mdp sans vérifier ancien
- Pas d'audit log

**Patch**:
- TODO - Vérifier ancien mot de passe avant modification
- TODO - Logger changement dans audit

---

### 5. `DELETE /api/vault/{vaultId}`
**Faille**:
- Suppression sans confirmation (seulement front)
- Pas de soft delete

**Patch**:
- TODO - Ajouter confirmation token
- TODO - Implémenter soft delete (IsDeleted flag)

---

### 6. `GET /api/vault/{vaultId}/users`
**Faille**:
- Pas de pagination

**Patch**:
- TODO - Ajouter params `page` et `pageSize`

---

### 7. `POST /api/vault/{vaultId}/share`
**Faille**:
- Énumération utilisateurs possible
- Messages différenciés

**Patch**:
- TODO - Ajouter `[EnableRateLimiting("share")]`
- TODO - Retourner message générique même si user n'existe pas

---

### 8. `DELETE /api/vault/{vaultId}/share/{targetUserId}`
**Faille**:
- Propriétaire peut se retirer lui-même (seulement vérifier côté front)

**Patch**:
- TODO - Vérifier que targetUserId != vault.UserId

---

## VaultSessionController

### 9. `POST /api/vaults/{vaultId}/unlock`
**Faille**:
- Timing attack
- Messages différenciés

**Patch**:
- TODO - Temps de réponse constant (200ms min)
- TODO - Message unique quelque soit l'erreur

---

### 10. `POST /api/vaults/{vaultId}/session/store-key`
**Faille**:
- Singleton stocke clés en clair
- IP falsifiable

**Patch**:
- TODO - Remplacer par Redis + Data Protection API
- TODO - Utiliser service pour récupérer IP sécurisée

---

### 11. `GET /api/vaults/{vaultId}/session/key`
**Faille**:
- Même problème Singleton
- Timeout hardcodé (30min)

**Patch**:
- TODO - Utiliser Redis
- TODO - Timeout configurable dans appsettings.json

---

## VaultEntryController

### 13. `POST /api/vaults/{vaultId}/entries/create`
**Faille**:
- Spam possible (pas de limite)
- IP falsifiable

**Patch**:
- TODO - Ajouter `[EnableRateLimiting("create")]`
- TODO - Limiter nb entrées par coffre (ex: 10000)

---

### 14. `GET /api/vaults/{vaultId}/entries/metadata`
**Faille**: Pas de pagination  
**Patch**: TODO - Ajouter `page` et `pageSize` (max 100)

---

### 16. `DELETE /api/vaults/{vaultId}/entries/{entryId}`
**Faille**: 
- Pas de confirmation (seulement front)
- Pas de soft delete

**Patch**:
- TODO - Soft delete
- TODO - Confirmation avant suppression

---

### 17. `PUT /api/vaults/{vaultId}/entries/{entryId}`
**Faille**: Pas de versioning  
**Patch**: TODO - Table EntryVersion pour historique

---

## UserLogsController

### 18. `GET /api/users/{userId}/logs`
**Faille**: 
- Dépend de `IAdminAuthorizationService` (custom)
- Pas de rate limiting

**Patch**:
- TODO - Utiliser `[Authorize(Roles = "Admin")]` natif ASP.NET
- TODO - Rate limiting pour admins

---
