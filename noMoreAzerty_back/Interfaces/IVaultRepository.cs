using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.Repositories
{
    public interface IVaultRepository
    {
        Task<IEnumerable<Vault>> GetAllVaultsAsync();
        Task<IEnumerable<Vault>> GetVaultsByUserAsync(Guid userId);
        Task<IEnumerable<Vault>> GetSharedVaultsAsync(Guid userId);
        Task<bool> VaultExistsAsync(Guid vaultId);
        Task<bool> UserHasAccessToVaultAsync(Guid vaultId, Guid userId);
        Task<Vault?> GetByIdAsync(Guid vaultId);
        Task<bool> IsVaultSharedWithUserAsync(Guid vaultId, Guid userId);
        Task AddAsync(Vault vault);
    }
}

