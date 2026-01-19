using Microsoft.EntityFrameworkCore;
using noMoreAzerty_back.Data;
using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.Repositories
{
    public class VaultEntryRepository : IVaultEntryRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public VaultEntryRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<VaultEntry>> GetEntriesByVaultAsync(Guid vaultId)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.VaultEntries
                .AsNoTracking()
                .Where(e => e.VaultId == vaultId && (e.IsActive ?? true))
                .Select(e => new VaultEntry
                {
                    Id = e.Id,

                    CipherTitle = e.CipherTitle,
                    TitleIV = e.TitleIV,
                    TitleTag = e.TitleTag,

                    CipherUsername = e.CipherUsername,
                    UsernameIV = e.UsernameIV,
                    UsernameTag = e.UsernameTag,

                    CipherPassword = e.CipherPassword,
                    PasswordIV = e.PasswordIV,
                    PasswordTag = e.PasswordTag,

                    CipherUrl = e.CipherUrl,
                    UrlIV = e.UrlIV,
                    UrlTag = e.UrlTag,

                    CipherCommentary = e.CipherCommentary,
                    ComentaryIV = e.ComentaryIV,
                    ComentaryTag = e.ComentaryTag,

                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<VaultEntry?> GetByIdAsync(Guid entryId)
        {
            await using var context = _contextFactory.CreateDbContext();
            return await context.VaultEntries
                .FirstOrDefaultAsync(e =>
                    e.Id == entryId &&
                    (e.IsActive ?? true));
        }

        public async Task AddAsync(VaultEntry entry)
        {
            await using var context = _contextFactory.CreateDbContext();
            await context.VaultEntries.AddAsync(entry);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(VaultEntry entry)
        {
            await using var context = _contextFactory.CreateDbContext();
            context.VaultEntries.Update(entry);
            await context.SaveChangesAsync();
        }
    }
}
