using Microsoft.EntityFrameworkCore;
using noMoreAzerty_back.Data;
using noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public UserRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
            return await context.Users.FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> GetByNameAsync(string name)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
            return await context.Users.FirstOrDefaultAsync(u => u.Name == name);
        }


        public async Task AddAsync(User user)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task<List<User>> GetAllAsync()
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
            return await context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> IsUserAdminAsync(Guid userId)
        {
            await using AppDbContext context = _contextFactory.CreateDbContext();
            return await context.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.Role == "Admin");
        }

        public async Task<IEnumerable<(User User, bool IsOwner, DateTime SharedAt)>> GetVaultUsersAsync(Guid vaultId)
        {
            await using var context = _contextFactory.CreateDbContext();

            var vault = await context.Vaults
                .Include(v => v.User)
                .Include(v => v.Shares)
                .ThenInclude(s => s.User)
                .FirstOrDefaultAsync(v => v.Id == vaultId);

            if (vault == null)
                return Enumerable.Empty<(User, bool, DateTime)>();

            var users = new List<(User User, bool IsOwner, DateTime SharedAt)>();

            // Ajouter le propriétaire
            if (vault.User != null)
            {
                users.Add((vault.User, true, vault.CreatedAt));
            }

            // Ajouter les utilisateurs partagés
            foreach (var share in vault.Shares.Where(s => s.User != null))
            {
                users.Add((share.User!, false, share.AddedAt));
            }

            return users;
        }
    }
}
