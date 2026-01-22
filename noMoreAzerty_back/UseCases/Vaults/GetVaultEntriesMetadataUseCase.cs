using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Models;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_back.Services;
using noMoreAzerty_dto.DTOs.Response;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class GetVaultEntriesMetadataUseCase
    {
        private readonly IVaultRepository _vaultRepository;
        private readonly IVaultEntryRepository _vaultEntryRepository;

        public GetVaultEntriesMetadataUseCase(
            IVaultRepository vaultRepository,
            IVaultEntryRepository vaultEntryRepository)
        {
            _vaultRepository = vaultRepository;
            _vaultEntryRepository = vaultEntryRepository;
        }

        public async Task<IReadOnlyList<VaultEntryMetadataResponse>> ExecuteAsync(
            Guid vaultId,
            Guid userId,
            string userIp)
        {
            // Vérifier que le coffre existe
            Vault? vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                throw new NotFoundException("Vault not found.");

            // Vérifier que l'utilisateur a accès au coffre
            if (!await _vaultRepository.UserHasAccessToVaultAsync(vaultId, userId))
                throw new ForbiddenException("User does not have access to this vault.");

            // Vérifier la session avec VaultSessionManager
            var sessionManager = VaultSessionManager.Instance;
            var keyStorage = sessionManager.GetKeyStorage(userId, vaultId, userIp, TimeSpan.FromMinutes(30));

            if (keyStorage == null)
            {
                throw new ForbiddenException("No valid vault session. Please unlock vault first.");
            }

            // Récupérer les entrées
            List<VaultEntry> entries = await _vaultEntryRepository.GetEntriesByVaultAsync(vaultId);

            // Retourner uniquement les métadonnées (titre + id)
            return entries.Select(e => new VaultEntryMetadataResponse
            {
                Id = e.Id,
                CipherTitle = e.CipherTitle,
                TitleIV = e.TitleIV,
                TitleTag = e.TitleTag,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            }).ToList();
        }
    }
}
