using noMoreAzerty_back.Repositories;
using noMoreAzerty_dto.DTOs.Response;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class GetSharedVaultsUseCase
    {
        private readonly IVaultRepository _vaultRepository;

        public GetSharedVaultsUseCase(IVaultRepository vaultRepository)
        {
            _vaultRepository = vaultRepository;
        }

        public async Task<IEnumerable<GetVaultResponse>> ExecuteAsync(Guid userId)
        {
            var sharedVaults = await _vaultRepository.GetSharedVaultsAsync(userId);

            return sharedVaults.Select(v => new GetVaultResponse
            {
                Id = v.Id,
                Name = v.Name,
                CreatedAt = v.CreatedAt,
                User = new VaultUserResponse
                {
                    Id = v.UserId
                }
            });
        }
    }
}
