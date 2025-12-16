using noMoreAzerty_back.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace noMoreAzerty_back.Repositories
{
    public interface IVaultRepository
    {
        Task<IEnumerable<Vault>> GetAllVaultsAsync();
        Task<IEnumerable<Vault>> GetVaultsByUserAsync(Guid userId);
        Task<IEnumerable<Vault>> GetSharedVaultsAsync(Guid userId);
        Task AddAsync(Vault vault);
    }

}