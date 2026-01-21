using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.Interfaces
{
    namespace noMoreAzerty_back.Interfaces
    {
        public interface IVaultEntryHistoryRepository
        {
            Task AddAsync(VaultEntryHistory history);
            Task<VaultEntryHistory?> GetByIdAsync(Guid id);
            Task<IEnumerable<VaultEntryHistory>> GetByVaultIdAsync(Guid vaultId);
        }
    }
}
