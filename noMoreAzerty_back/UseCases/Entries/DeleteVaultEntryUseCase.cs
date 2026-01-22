using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Interfaces.Services;
using noMoreAzerty_back.Models.Enums;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_back.Services;

namespace noMoreAzerty_back.UseCases.Entries
{
    public class DeleteVaultEntryUseCase
    {
        private readonly IVaultEntryRepository _vaultEntryRepository;
        private readonly IVaultRepository _vaultRepository;
        private readonly IVaultEntryHistoryService _vaultEntryHistoryService;

        public DeleteVaultEntryUseCase(
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
            string userIp)
        {
            // Vérifier que le coffre existe
            var vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                throw new NotFoundException("Vault not found");

            // Vérifier que l'utilisateur a accès au coffre (propriétaire ou partagé)
            bool isOwner = vault.UserId == userId;
            bool isShared = await _vaultRepository.IsVaultSharedWithUserAsync(vaultId, userId);

            if (!isOwner && !isShared)
                throw new ForbiddenException("User does not have access to this vault");

            // Vérifier la session avec VaultSessionManager
            var sessionManager = VaultSessionManager.Instance;
            var keyStorage = sessionManager.GetKeyStorage(userId, vaultId, userIp, TimeSpan.FromMinutes(30));

            if (keyStorage == null)
            {
                throw new ForbiddenException("No valid vault session. Please unlock vault first.");
            }

            // Vérifier que l'entrée existe
            var entry = await _vaultEntryRepository.GetByIdAsync(entryId);
            if (entry == null)
                throw new NotFoundException("Vault entry not found");

            // Vérifier que l'entrée appartient bien au coffre
            if (entry.VaultId != vaultId)
                throw new ForbiddenException("Entry does not belong to this vault");

            // Journalisation basique de la suppression
            await _vaultEntryHistoryService.LogEntryCreatedAsync(
                VaultEntryAction.Deleted,
                userId: userId,
                vaultId: vaultId,
                entry: entry
            );

            // Soft delete
            entry.IsActive = false;
            entry.UpdatedAt = DateTime.UtcNow;


            // Journalisation basique de la suppression
            await _vaultEntryHistoryService.LogEntryCreatedAsync(
                VaultEntryAction.Deleted,
                userId: userId,
                vaultId: vaultId,
                entry: entry
            );

            await _vaultEntryRepository.UpdateAsync(entry);
        }
    }
}
