using Microsoft.EntityFrameworkCore;
using noMoreAzerty_back.Data;
using noMoreAzerty_back.Interfaces.noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.Repositories
{
    public class VaultEntryHistoryRepository : IVaultEntryHistoryRepository
    {
        private readonly AppDbContext _context;

        public VaultEntryHistoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(VaultEntryHistory history)
        {
            await _context.VaultEntryHistories.AddAsync(history);
        }

        public async Task<VaultEntryHistory?> GetByIdAsync(Guid id)
        {
            return await _context.VaultEntryHistories
                .FirstOrDefaultAsync(h => h.Id == id);
        }

        public async Task<IEnumerable<VaultEntryHistory>> GetByVaultIdAsync(Guid vaultId)
        {
            return await _context.VaultEntryHistories
                .Where(h => h.VaultId == vaultId)
                .OrderByDescending(h => h.CreatedAt)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}