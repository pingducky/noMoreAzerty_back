using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Interfaces.Services;
using noMoreAzerty_back.Models.Enums;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_back.Services;

namespace noMoreAzerty_back.UseCases.Entries
{
    public class UpdateVaultEntryUseCase
    {
        private readonly IVaultEntryRepository _vaultEntryRepository;
        private readonly IVaultRepository _vaultRepository;
        private readonly IVaultEntryHistoryService _vaultEntryHistoryService;

        public UpdateVaultEntryUseCase(
            IVaultEntryRepository vaultEntryRepository,
            IVaultRepository vaultRepository,
            IVaultEntryHistoryService vaultEntryHistoryService)
        {
            _vaultEntryRepository = vaultEntryRepository;
            _vaultRepository = vaultRepository;
            _vaultEntryHistoryService = vaultEntryHistoryService;
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
            // Vérifier que le coffre existe
            var vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                throw new NotFoundException("Vault not found");

            // Vérifier que l'utilisateur est owner
            if (vault.UserId != userId)
                throw new ForbiddenException("User is not owner of the vault");

            // Vérifier la session RAM
            var sessionManager = VaultSessionManager.Instance;
            if (!sessionManager.HasRecentSession(
                    userId,
                    vaultId,
                    userIp,
                    TimeSpan.FromMinutes(10)))
            {
                throw new ForbiddenException("No valid vault session");
            }

            // Vérifier que l'entrée existe
            var entry = await _vaultEntryRepository.GetByIdAsync(entryId);
            if (entry == null)
                throw new NotFoundException("Vault entry not found");

            // Vérifier que l'entrée appartient bien au coffre
            if (entry.VaultId != vaultId)
                throw new ForbiddenException("Entry does not belong to this vault");

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

            // Journalisation basique de l'update
            await _vaultEntryHistoryService.LogEntryCreatedAsync(
                VaultEntryAction.Read,
                userId: userId,
                vaultId: vaultId,
                entry: entry
            );

            await _vaultEntryRepository.UpdateAsync(entry);
        }
    }
}
