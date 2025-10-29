using noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Models;

namespace MyApiProject.UseCases.Users
{
    public class GetOrCreateCurrentUserUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetOrCreateCurrentUserUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<User> ExecuteAsync(Guid userGuid)
        {
            var user = await _userRepository.GetByIdAsync(userGuid);

            if (user == null)
            {
                user = new User
                {
                    Id = userGuid,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();
            }

            return user;
        }
    }
}
