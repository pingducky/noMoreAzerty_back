using noMoreAzerty_back.Models;

public interface IVaultEntryHistoryRepository
{
    Task AddAsync(VaultEntryHistory history);
    Task<VaultEntryHistory?> GetByIdAsync(Guid id);
    Task<IEnumerable<VaultEntryHistory>> GetByVaultIdAsync(Guid vaultId);
}
