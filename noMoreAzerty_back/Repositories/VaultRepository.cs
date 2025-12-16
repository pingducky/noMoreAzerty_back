using Microsoft.EntityFrameworkCore;
using noMoreAzerty_back.Data;
using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.Repositories
{
    public class VaultRepository : IVaultRepository
    {
        private readonly AppDbContext _context;

        public VaultRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vault>> GetAllVaultsAsync()
        {
            return await _context.Vaults.Include(v => v.User).ToListAsync();
        }

        public async Task<IEnumerable<Vault>> GetVaultsByUserAsync(Guid userId)
        {
            return await _context.Vaults
                .Include(v => v.User)
                .Where(v => v.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vault>> GetSharedVaultsAsync(Guid userId)
        {
            return await _context.Shares
                .Include(s => s.Vault)
                    .ThenInclude(v => v.User)
                .Where(s => s.UserId == userId)
                .Select(s => s.Vault!)
                .ToListAsync();
        }

        public async Task AddAsync(Vault vault)
        {
            _context.Vaults.Add(vault);
            await _context.SaveChangesAsync();
        }
    }
}
