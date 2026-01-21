using Microsoft.EntityFrameworkCore;
using noMoreAzerty_back.Data;
using noMoreAzerty_back.Interfaces.noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Models;
using noMoreAzerty_back.Models.Enums;

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
            await using AppDbContext context = _contextFactory.CreateDbContext();
            await context.VaultEntryHistory.AddAsync(history); 
            await context.SaveChangesAsync();
        }

        public async Task<VaultEntryHistory?> GetByIdAsync(Guid id)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
            return await context.VaultEntryHistory
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<IEnumerable<VaultEntryHistory>> GetByVaultIdAsync(Guid vaultId)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
            return await context.VaultEntryHistory
                .Where(h => h.VaultId == vaultId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task<(IReadOnlyList<VaultEntryHistory> Items, int TotalCount)>
        GetByUserWithFiltersAsync(
            Guid userId,
            VaultEntryAction[]? actions,
            int page,
            int pageSize)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();

            var query = context.VaultEntryHistory
                .Where(h => h.UserId == userId);

            if (actions is { Length: > 0 })
            {
                query = query.Where(h => actions.Contains(h.Action));
            }

            int totalCount = await query.CountAsync();

            List<VaultEntryHistory> items = await query
                .OrderByDescending(h => h.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}

