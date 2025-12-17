using noMoreAzerty_back.Models;
using noMoreAzerty_back.Repositories;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class CreateVaultUseCase
    {
        private readonly IVaultRepository _vaultRepository;

        public CreateVaultUseCase(IVaultRepository vaultRepository)
        {
            _vaultRepository = vaultRepository;
        }

        public async Task<Guid> ExecuteAsync(
            Guid userId,
            string name,
            string derivedPassword,
            string passwordSalt)
        {
            // Concaténation mot de passe + sel client
            var passwordToHash = $"{derivedPassword}{passwordSalt}";

            // Hash BCrypt
            var bcryptHash = BCrypt.Net.BCrypt.HashPassword(passwordToHash);

            var vault = new Vault
            {
                Id = Guid.NewGuid(),
                Name = name,
                HashPassword = bcryptHash,
                PasswordSalt = passwordSalt,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _vaultRepository.AddAsync(vault);

            return vault.Id;
        }
    }
}
