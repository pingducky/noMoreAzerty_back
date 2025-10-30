using noMoreAzerty_back.Models;
using noMoreAzerty_back.Repositories;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class GetSharedVaultsUseCase
    {
        private readonly IVaultRepository _vaultRepository;

        public GetSharedVaultsUseCase(IVaultRepository vaultRepository)
        {
            _vaultRepository = vaultRepository;
        }

        public async Task<IEnumerable<object>> ExecuteAsync(Guid userId)
        {
            var sharedVaults = await _vaultRepository.GetSharedVaultsAsync(userId);

            return sharedVaults.Select(v => new
            {
                v.Id,
                v.Name,
                v.CreatedAt,
                Owner = new
                {
                    v.UserId,
                }
            });
        }
    }
}
