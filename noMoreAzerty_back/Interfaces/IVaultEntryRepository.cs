using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.Repositories
{
    public interface IVaultEntryRepository
    {
        Task<List<VaultEntry>> GetEntriesByVaultAsync(Guid vaultId);
        Task<VaultEntry?> GetByIdAsync(Guid entryId);
        Task AddAsync(VaultEntry entry);
        Task UpdateAsync(VaultEntry entry);
    }
}
