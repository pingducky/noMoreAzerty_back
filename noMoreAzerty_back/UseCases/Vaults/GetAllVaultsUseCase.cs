using noMoreAzerty_back.Models;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_dto.Dtos.Request;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class GetAllVaultsUseCase
    {
        private readonly IVaultRepository _vaultRepository;

        public GetAllVaultsUseCase(IVaultRepository vaultRepository)
        {
            _vaultRepository = vaultRepository;
        }

        public async Task<IEnumerable<Vault>> ExecuteAsync()
        {
            return await _vaultRepository.GetAllVaultsAsync();
        }
    }
}