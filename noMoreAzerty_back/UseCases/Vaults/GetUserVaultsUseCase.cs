using noMoreAzerty_back.Models;
using noMoreAzerty_back.Repositories;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class GetUserVaultsUseCase
    {
        private readonly IVaultRepository _vaultRepository;

        public GetUserVaultsUseCase(IVaultRepository vaultRepository)
        {
            _vaultRepository = vaultRepository;
        }

        public async Task<IEnumerable<object>> ExecuteAsync(Guid userId)
        {
            var vaults = await _vaultRepository.GetVaultsByUserAsync(userId);

            return vaults.Select(v => new
            {
                // Informations du coffre
                v.Id,
                v.Name,
                v.CreatedAt,

                // Informations de l'utilisateur
                User = new
                {
                    v.UserId,
                }
            });
        }
    }
}
