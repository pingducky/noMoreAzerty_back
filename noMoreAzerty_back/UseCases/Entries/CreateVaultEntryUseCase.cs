using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Interfaces.Services;
using noMoreAzerty_back.Models;
using noMoreAzerty_back.Models.Enums;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_back.Services;
using noMoreAzerty_dto.DTOs.Response;

namespace noMoreAzerty_back.UseCases.Entries
{
    public class CreateVaultEntryUseCase
    {
        private readonly IVaultEntryRepository _vaultEntryRepository;
        private readonly IVaultRepository _vaultRepository;
        private readonly IVaultEntryHistoryService _vaultEntryHistoryService;

        public CreateVaultEntryUseCase(
            IVaultEntryRepository vaultEntryRepository,
            IVaultRepository vaultRepository,
            IVaultEntryHistoryService vaultEntryHistoryService)
        {
            _vaultEntryRepository = vaultEntryRepository;
            _vaultRepository = vaultRepository;
            _vaultEntryHistoryService = vaultEntryHistoryService;
        }

        public async Task<GetVaultEntriesResponse> ExecuteAsync(
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

            // Création de l'entrée (données déjà chiffrées)
            VaultEntry entry = new VaultEntry
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

            // Journalisation basique de la création
            await _vaultEntryHistoryService.LogEntryCreatedAsync(
                VaultEntryAction.Created,
                userId: userId,
                vaultId: vaultId,
                entry: entry
            );

            GetVaultEntriesResponse vaultEntry = new GetVaultEntriesResponse
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

            return vaultEntry;
        }
    }
}
