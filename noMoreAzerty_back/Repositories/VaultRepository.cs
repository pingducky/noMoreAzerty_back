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

        public async Task<bool> VaultExistsAsync(Guid vaultId)
        {
            return await _context.Vaults.AnyAsync(v => v.Id == vaultId);
        }

        public async Task<bool> UserHasAccessToVaultAsync(Guid vaultId, Guid userId)
        {
            // Vérifie si l'utilisateur est propriétaire
            bool isOwner = await _context.Vaults
                .AnyAsync(v => v.Id == vaultId && v.UserId == userId);

            if (isOwner)
                return true;

            // Vérifie si l'utilisateur a un partage
            bool isShared = await _context.Shares
                .AnyAsync(s => s.VaultId == vaultId && s.UserId == userId); 

            return isShared;
        }

        public async Task<Vault?> GetByIdAsync(Guid vaultId)
        {
            return await _context.Vaults.FirstOrDefaultAsync(v => v.Id == vaultId);
        }

        public async Task<bool> IsVaultSharedWithUserAsync(Guid vaultId, Guid userId)
        {
            return await _context.Shares
                .AnyAsync(s => s.VaultId == vaultId && s.UserId == userId);
        }

    }
}
