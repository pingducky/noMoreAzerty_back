using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_back.Services;
using static noMoreAzerty_back.Controllers.VaultEntryController;

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
            var vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                throw new NotFoundException("Vault not found.");

            // Vérifier que l'utilisateur a accès au coffre
            if (!await _vaultRepository.UserHasAccessToVaultAsync(vaultId, userId))
                throw new ForbiddenException("User does not have access to this vault.");

            // Vérifier la session
            var sessionManager = VaultSessionManager.Instance;
            if (!sessionManager.HasRecentSession(userId, vaultId, userIp, TimeSpan.FromMinutes(10)))
            {
                throw new ForbiddenException("No valid vault session. Please authenticate first.");
            }

            // Récupérer les entrées
            var entries = await _vaultEntryRepository.GetEntriesByVaultAsync(vaultId);

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
