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
    }
}
