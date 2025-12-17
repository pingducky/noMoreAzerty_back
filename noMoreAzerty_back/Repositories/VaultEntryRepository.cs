using Microsoft.EntityFrameworkCore;
using noMoreAzerty_back.Data;
using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.Repositories
{
    public class VaultEntryRepository : IVaultEntryRepository
    {
        private readonly AppDbContext _context;

        public VaultEntryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<VaultEntry>> GetEntriesByVaultAsync(Guid vaultId)
        {
            return await _context.VaultEntries
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
    }
}
