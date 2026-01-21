using noMoreAzerty_back.Interfaces.Services;
using noMoreAzerty_back.Models;
using noMoreAzerty_back.Models.Enums;
using noMoreAzerty_back.Interfaces;

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
                Action = action,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                VaultId = vaultId,
                EntryId = entry.Id
            };

            await _historyRepository.AddAsync(history);
        }

    }
}
