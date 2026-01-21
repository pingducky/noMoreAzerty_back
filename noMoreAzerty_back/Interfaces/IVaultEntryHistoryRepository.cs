using noMoreAzerty_back.Models;
using noMoreAzerty_back.Models.Enums;

namespace noMoreAzerty_back.Interfaces
{
    namespace noMoreAzerty_back.Interfaces
    {
        public interface IVaultEntryHistoryRepository
        {
            Task AddAsync(VaultEntryHistory history);
            Task<VaultEntryHistory?> GetByIdAsync(Guid id);
            Task<IEnumerable<VaultEntryHistory>> GetByVaultIdAsync(Guid vaultId);
            Task<(IReadOnlyList<VaultEntryHistory> Items, int TotalCount)>
            GetByUserWithFiltersAsync(
                Guid userId,
                VaultEntryAction[]? actions,
                int page,
                int pageSize);
                    }
    }
}
