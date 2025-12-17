using noMoreAzerty_back.Models;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_back.Services;

namespace noMoreAzerty_back.UseCases.Entries
{
    public class CreateVaultEntryUseCase
    {
        private readonly IVaultEntryRepository _vaultEntryRepository;
        private readonly IVaultRepository _vaultRepository;

        public CreateVaultEntryUseCase(
            IVaultEntryRepository vaultEntryRepository,
            IVaultRepository vaultRepository)
        {
            _vaultEntryRepository = vaultEntryRepository;
            _vaultRepository = vaultRepository;
        }

        public async Task<Guid> ExecuteAsync(
            Guid userId,
            Guid vaultId,
            string userIp,
            string? cipherTitle,
            string? titleIV,
            string? titleTag,
            string? cipherUsername,
            string? usernameIV,
            string? usernameTag,
            string? cipherPassword,
            string? passwordIV,
            string? passwordTag,
            string? cipherUrl,
            string? urlIV,
            string? urlTag,
            string? cipherCommentary,
            string? comentaryIV,
            string? comentaryTag)
        {
            // Vérifier que le coffre existe
            var vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                throw new KeyNotFoundException("Vault not found");

            // Interdire explicitement les coffres partagés (lecture seule)
            bool isShared = await _vaultRepository.IsVaultSharedWithUserAsync(vaultId, userId);
            if (isShared)
                throw new UnauthorizedAccessException("Shared vaults are read-only");

            // Vérifier que l'utilisateur est propriétaire du coffre
            if (vault.UserId != userId)
                throw new UnauthorizedAccessException("User is not owner of the vault");

            // Vérifier que l'user s'est authentifier récemment (10 minutes) & mettre à jour la session RAM
            var sessionManager = VaultSessionManager.Instance;
            var maxAge = TimeSpan.FromMinutes(10);

            if (!sessionManager.HasRecentSession(userId, vaultId, userIp, maxAge))
            {
                sessionManager.UpdateSession(userId, vaultId, userIp);
            }

            // Création de l'entrée (données déjà chiffrées)
            var entry = new VaultEntry
            {
                Id = Guid.NewGuid(),
                VaultId = vaultId,
                CipherTitle = cipherTitle,
                TitleIV = titleIV,
                TitleTag = titleTag,
                CipherUsername = cipherUsername,
                UsernameIV = usernameIV,
                UsernameTag = usernameTag,
                CipherPassword = cipherPassword,
                PasswordIV = passwordIV,
                PasswordTag = passwordTag,
                CipherUrl = cipherUrl,
                UrlIV = urlIV,
                UrlTag = urlTag,
                CipherCommentary = cipherCommentary,
                ComentaryIV = comentaryIV,
                ComentaryTag = comentaryTag,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _vaultEntryRepository.AddAsync(entry);
            return entry.Id;
        }
    }
}
