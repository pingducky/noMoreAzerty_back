using noMoreAzerty_back.Repositories;
using noMoreAzerty_back.Services;

namespace noMoreAzerty_back.UseCases.Entries
{
    public class UpdateVaultEntryUseCase
    {
        private readonly IVaultEntryRepository _vaultEntryRepository;
        private readonly IVaultRepository _vaultRepository;

        public UpdateVaultEntryUseCase(
            IVaultEntryRepository vaultEntryRepository,
            IVaultRepository vaultRepository)
        {
            _vaultEntryRepository = vaultEntryRepository;
            _vaultRepository = vaultRepository;
        }

        public async Task ExecuteAsync(
            Guid userId,
            Guid vaultId,
            Guid entryId,
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
            // 1️ Vérifier que le coffre existe
            var vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                throw new KeyNotFoundException("Vault not found");

            // Vérifier que l'utilisateur est owner
            if (vault.UserId != userId)
                throw new UnauthorizedAccessException("User is not owner of the vault");

            // Vérifier la session RAM
            var sessionManager = VaultSessionManager.Instance;
            if (!sessionManager.HasRecentSession(
                    userId,
                    vaultId,
                    userIp,
                    TimeSpan.FromMinutes(10)))
            {
                throw new UnauthorizedAccessException("No valid vault session");
            }

            // Vérifier que l'entrée existe
            var entry = await _vaultEntryRepository.GetByIdAsync(entryId);
            if (entry == null)
                throw new KeyNotFoundException("Vault entry not found");

            // Vérifier que l'entrée appartient bien au coffre
            if (entry.VaultId != vaultId)
                throw new UnauthorizedAccessException("Entry does not belong to this vault");

            // Update
            entry.CipherTitle = cipherTitle;
            entry.TitleIV = titleIV;
            entry.TitleTag = titleTag;

            entry.CipherUsername = cipherUsername;
            entry.UsernameIV = usernameIV;
            entry.UsernameTag = usernameTag;

            entry.CipherPassword = cipherPassword;
            entry.PasswordIV = passwordIV;
            entry.PasswordTag = passwordTag;

            entry.CipherUrl = cipherUrl;
            entry.UrlIV = urlIV;
            entry.UrlTag = urlTag;

            entry.CipherCommentary = cipherCommentary;
            entry.ComentaryIV = comentaryIV;
            entry.ComentaryTag = comentaryTag;

            entry.UpdatedAt = DateTime.UtcNow;

            await _vaultEntryRepository.UpdateAsync(entry);
        }
    }
}
