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

        public async Task<IEnumerable<UserVaultReadResponse>> ExecuteAsync(Guid userId)
        {
            var vaults = await _vaultRepository.GetVaultsByUserAsync(userId);

            return vaults.Select(v => new UserVaultReadResponse
            {
                Id = v.Id,
                Name = v.Name,
                CreatedAt = v.CreatedAt,
                IsShared = false,
                PasswordSalt = v.PasswordSalt
            });
        }

        public class UserVaultReadResponse // Todo: utiliser la response de la librairie partagé
        {
            public Guid Id { get; set; }
            public string? Name { get; set; } = null!;
            public DateTime CreatedAt { get; set; }
            public bool IsShared { get; set; }
            public string? PasswordSalt { get; set; } = null!;
        }
    }
}
