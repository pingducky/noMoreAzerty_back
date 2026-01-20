using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.Interfaces
{
    public interface IVaultEntryHistoryRepository
    {
        Task AddAsync(VaultEntryHistory history);
    }
}
