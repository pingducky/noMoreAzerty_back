using noMoreAzerty_back.Interfaces;
using noMoreAzerty_dto.DTOs.Response;
using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.UseCases.Users
{
    public class GetUsersUseCase
    {
        private readonly IUserRepository _userRepository;

        public GetUsersUseCase(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserListItemResponse>> ExecuteAsync()
        {
            List<User> users = await _userRepository.GetAllAsync();

            return users.Select(u => new UserListItemResponse
            {
                Id = u.Id,
                CreatedAt = u.CreatedAt,
                LastLogin = u.LastLogin,
                IsActive = u.IsActive
            });
        }
    }
}
