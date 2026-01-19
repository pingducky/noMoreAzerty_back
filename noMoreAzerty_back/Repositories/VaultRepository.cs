using Microsoft.EntityFrameworkCore;
using noMoreAzerty_back.Data;
using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.Repositories
{
    public class VaultRepository : IVaultRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public VaultRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Vault>> GetAllVaultsAsync()
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Vaults.Include(v => v.User).ToListAsync();
        }

        public async Task<IEnumerable<Vault>> GetVaultsByUserAsync(Guid userId)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Vaults
                .Include(v => v.User)
                .Where(v => v.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vault>> GetSharedVaultsAsync(Guid userId)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Vaults
                .Include(v => v.User)
                .Where(v => v.Shares.Any(s => s.UserId == userId))
                .ToListAsync();
        }

        public async Task AddAsync(Vault vault)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.Vaults.Add(vault);
            await context.SaveChangesAsync();
        }

        public async Task<bool> VaultExistsAsync(Guid vaultId)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Vaults.AnyAsync(v => v.Id == vaultId);
        }

        public async Task<bool> UserHasAccessToVaultAsync(Guid vaultId, Guid userId)
        {
            await using var context = _contextFactory.CreateDbContext();
            // Vérifie si l'utilisateur est propriétaire
            bool isOwner = await context.Vaults
                .AnyAsync(v => v.Id == vaultId && v.UserId == userId);

            if (isOwner)
                return true;

            // Vérifie si l'utilisateur a un partage
            bool isShared = await context.Shares
                .AnyAsync(s => s.VaultId == vaultId && s.UserId == userId);

            return isShared;
        }

        public async Task<Vault?> GetByIdAsync(Guid vaultId)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Vaults.FirstOrDefaultAsync(v => v.Id == vaultId);
        }

        public async Task<bool> IsVaultSharedWithUserAsync(Guid vaultId, Guid userId)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.Shares
                .AnyAsync(s => s.VaultId == vaultId && s.UserId == userId);
        }
    }
}
