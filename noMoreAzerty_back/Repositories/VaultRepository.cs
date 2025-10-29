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
            return await _context.Vaults
                .Include(v => v.User)
                .ToListAsync();
        }
    }
}