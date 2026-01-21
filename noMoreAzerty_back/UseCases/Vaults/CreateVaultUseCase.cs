using noMoreAzerty_back.Models;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_dto.DTOs.Response;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class CreateVaultUseCase
    {
        private readonly IVaultRepository _vaultRepository;

        public CreateVaultUseCase(IVaultRepository vaultRepository)
        {
            _vaultRepository = vaultRepository;
        }

        public async Task<GetVaultResponse> ExecuteAsync(
            Guid userId,
            string name,
            string derivedPassword,
            string passwordSalt)
        {
            // Concaténation mot de passe + sel client
            String passwordToHash = $"{derivedPassword}{passwordSalt}";

            // Hash BCrypt
            String bcryptHash = BCrypt.Net.BCrypt.HashPassword(passwordToHash);

            Vault vault = new Vault
            {
                Id = Guid.NewGuid(),
                Name = name,
                HashPassword = bcryptHash,
                PasswordSalt = passwordSalt,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _vaultRepository.AddAsync(vault);

            return new GetVaultResponse
            {
                Id = vault.Id,
                Name = vault.Name,
                CreatedAt = vault.CreatedAt,
                User = new VaultUserResponse
                {
                    Id = vault.UserId,
                }
            };
        }
    }
}
