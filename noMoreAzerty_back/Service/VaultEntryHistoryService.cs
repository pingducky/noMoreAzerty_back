using noMoreAzerty_back.Interfaces.noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Interfaces.Services;
using noMoreAzerty_back.Models;
using noMoreAzerty_back.Models.Enums;

namespace noMoreAzerty_back.Service
{
    public class VaultEntryHistoryService : IVaultEntryHistoryService
    {
        private readonly IVaultEntryHistoryRepository _historyRepository;

        public VaultEntryHistoryService(IVaultEntryHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        public async Task LogEntryCreatedAsync(
            VaultEntryAction action,
            Guid userId,
            Guid vaultId,
            VaultEntry entry)
        {
            var history = new VaultEntryHistory
            {
                Id = Guid.NewGuid(),

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

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null,

                Action = action,

                VaultId = vaultId,
                UserId = userId,
                VaultEntryId = entry.Id
            };

            await _historyRepository.AddAsync(history);
        }
    }
}