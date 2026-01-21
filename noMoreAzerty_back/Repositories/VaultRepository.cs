using Microsoft.EntityFrameworkCore;
using noMoreAzerty_back.Data;
using noMoreAzerty_back.Exceptions;
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
            await using AppDbContext context = _contextFactory.CreateDbContext();
            return await context.Vaults.Include(v => v.User).ToListAsync();
        }

        public async Task<IEnumerable<Vault>> GetVaultsByUserAsync(Guid userId)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
            return await context.Vaults
                .Include(v => v.User)
                .Where(v => v.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vault>> GetSharedVaultsAsync(Guid userId)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
            return await context.Vaults
                .Include(v => v.User)
                .Where(v => v.Shares.Any(s => s.UserId == userId))
                .ToListAsync();
        }

        public async Task AddAsync(Vault vault)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
            context.Vaults.Add(vault);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vault vault)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
            context.Vaults.Update(vault);
            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid vaultId)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();

            // Démarrer une transaction pour garantir l'atomicité
            await using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // Vérifier que le coffre existe et que l'utilisateur en est le propriétaire
                Vault? vault = await context.Vaults.FindAsync(vaultId);

                if (vault == null)
                {
                    throw new NotFoundException("Le coffre n'existe pas.");
                }

                // Supprimer toutes les entrées du coffre
                List<VaultEntry>? entries = await context.VaultEntries
                    .Where(e => e.VaultId == vaultId)
                    .ToListAsync();

                if (entries.Any())
                {
                    context.VaultEntries.RemoveRange(entries);
                }

                // Supprimer les historiques d'entrées liés au coffre
                List<VaultEntryHistory>? histories = await context.VaultEntryHistory
                    .Where(h => h.VaultId == vaultId)
                    .ToListAsync();

                if (histories.Any())
                {
                    context.VaultEntryHistory.RemoveRange(histories);
                }

                // Supprimer les partages du coffre
                List<Share>? shares = await context.Shares
                    .Where(s => s.VaultId == vaultId)
                    .ToListAsync();

                if (shares.Any())
                {
                    context.Shares.RemoveRange(shares);
                }

                // Supprimer le coffre lui-même
                context.Vaults.Remove(vault);

                // Sauvegarder toutes les modifications
                await context.SaveChangesAsync();

                // Valider la transaction
                await transaction.CommitAsync();
            }
            catch (Exception)
            {
                // En cas d'erreur, annuler la transaction
                await transaction.RollbackAsync();
                throw; // Relancer l'exception pour la gestion au niveau supérieur
            }
        }

        public async Task<bool> VaultExistsAsync(Guid vaultId)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
            return await context.Vaults.AnyAsync(v => v.Id == vaultId);
        }

        public async Task<bool> UserHasAccessToVaultAsync(Guid vaultId, Guid userId)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
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
            await using AppDbContext context = _contextFactory.CreateDbContext();
            return await context.Vaults.FirstOrDefaultAsync(v => v.Id == vaultId);
        }

        public async Task<bool> IsVaultSharedWithUserAsync(Guid vaultId, Guid userId)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
            return await context.Shares
                .AnyAsync(s => s.VaultId == vaultId && s.UserId == userId);
        }

        public async Task ShareVaultAsync(Guid vaultId, Guid userId)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
            Share share = new Share
            {
                VaultId = vaultId,
                UserId = userId,
                AddedAt = DateTime.UtcNow
            };

            context.Shares.Add(share);
            await context.SaveChangesAsync();
        }

        public async Task UnshareVaultAsync(Guid vaultId, Guid userId)
        {
            await using var context = _contextFactory.CreateDbContext();
            Share? share = await context.Shares
                .FirstOrDefaultAsync(s => s.VaultId == vaultId && s.UserId == userId);

            if (share != null)
            {
                context.Shares.Remove(share);
                await context.SaveChangesAsync();
            }
        }

        public class ShareVaultRequest
        {
            public Guid TargetUserId { get; set; }
        }
    }
}
