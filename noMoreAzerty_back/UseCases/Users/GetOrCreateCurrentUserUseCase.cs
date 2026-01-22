using noMoreAzerty_back.Data;
using noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.UseCases.Users
{
    public class GetOrCreateCurrentUserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;

        public GetOrCreateCurrentUserUseCase(
            IUserRepository userRepository,
            AppDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
        }

        public async Task<User> ExecuteAsync(Guid userGuid, string? name)
        {
            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userRepository.GetByIdAsync(userGuid);

                if (user == null)
                {
                    user = new User
                    {
                        Id = userGuid,
                        Name = name,
                        CreatedAt = DateTime.UtcNow,
                        IsActive = true
                    };

                    await _userRepository.AddAsync(user);
                }

                await transaction.CommitAsync();
                return user;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
