using noMoreAzerty_back.Repositories;
using noMoreAzerty_dto.DTOs.Response;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class GetUserVaultsUseCase
    {
        private readonly IVaultRepository _vaultRepository;

        public GetUserVaultsUseCase(IVaultRepository vaultRepository)
        {
            _vaultRepository = vaultRepository;
        }

        public async Task<IEnumerable<GetVaultResponse>> ExecuteAsync(Guid userId)
        {
            var vaults = await _vaultRepository.GetVaultsByUserAsync(userId);

            return vaults.Select(v => new GetVaultResponse
            {
                Id = v.Id,
                Name = v.Name,
                PasswordSalt = v.PasswordSalt,
                CreatedAt = v.CreatedAt,
                User = new VaultUserResponse
                {
                    Id = v.UserId,
                }
            });
        }
    }
}
