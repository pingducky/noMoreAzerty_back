using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id);
        Task<User?> GetByNameAsync(string name);
        Task AddAsync(User user);
        Task<List<User>> GetAllAsync();
        Task<bool> IsUserAdminAsync(Guid userId);
        Task<IEnumerable<(User User, bool IsOwner, DateTime SharedAt)>> GetVaultUsersAsync(Guid vaultId);
    }
}
