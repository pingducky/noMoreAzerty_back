using Microsoft.EntityFrameworkCore;
using noMoreAzerty_back.Data;
using noMoreAzerty_back.Interfaces.noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.Repositories
{
    public class VaultEntryHistoryRepository : IVaultEntryHistoryRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public VaultEntryHistoryRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task AddAsync(VaultEntryHistory history)
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.VaultEntryHistory.AddAsync(history); 
            await context.SaveChangesAsync();
        }

        public async Task<VaultEntryHistory?> GetByIdAsync(Guid id)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.VaultEntryHistory
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<IEnumerable<VaultEntryHistory>> GetByVaultIdAsync(Guid vaultId)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.VaultEntryHistory
                .Where(h => h.VaultId == vaultId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }
    }
}
