using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_back.Services;
using noMoreAzerty_dto.DTOs.Response;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class GetVaultEntryByIdUseCase
    {
        private readonly IVaultRepository _vaultRepository;
        private readonly IVaultEntryRepository _vaultEntryRepository;

        public GetVaultEntryByIdUseCase(
            IVaultRepository vaultRepository,
            IVaultEntryRepository vaultEntryRepository)
        {
            _vaultRepository = vaultRepository;
            _vaultEntryRepository = vaultEntryRepository;
        }

        public async Task<GetVaultEntriesResponse> ExecuteAsync(
            Guid vaultId,
            Guid entryId,
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

            // Récupérer l'entrée
            var entry = await _vaultEntryRepository.GetByIdAsync(entryId);

            if (entry == null)
                throw new NotFoundException("Entry not found.");

            // Vérifier que l'entrée appartient bien au coffre
            if (entry.VaultId != vaultId)
                throw new ForbiddenException("Entry does not belong to this vault.");

            // Retourner l'entrée complète
            return new GetVaultEntriesResponse
            {
                Id = entry.Id,
                CipherTitle = entry.CipherTitle,
                TitleIV = entry.TitleIV,
                TitleTag = entry.TitleTag,
                CipherUsername = entry.CipherUsername,
                UsernameIV = entry.UsernameIV,
                UsernameTag = entry.UsernameTag,
                CipherPassword = entry.CipherPassword,
                PasswordIV = entry.PasswordIV,
                PasswordTag = entry.PasswordTag,
                CipherUrl = entry.CipherUrl,
                UrlIV = entry.UrlIV,
                UrlTag = entry.UrlTag,
                CipherCommentary = entry.CipherCommentary,
                ComentaryIV = entry.ComentaryIV,
                ComentaryTag = entry.ComentaryTag,
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt
            };
        }
    }
}
