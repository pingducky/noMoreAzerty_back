using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Interfaces.Services;
using noMoreAzerty_back.Models.Enums;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_dto.DTOs.Response;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class UpdateVaultUseCase
    {
        private readonly IVaultRepository _vaultRepository;

        public UpdateVaultUseCase(IVaultRepository vaultRepository)
        {
            _vaultRepository = vaultRepository;
        }

        public async Task<GetVaultResponse> ExecuteAsync(
            Guid vaultId,
            Guid userId,
            string newName,
            string? newDerivedPassword = null,
            string? passwordSalt = null)
        {
            // Vérifier que le coffre existe
            var vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                throw new NotFoundException("Le coffre n'existe pas.");

            // Vérifier que l'utilisateur est bien le propriétaire
            if (vault.UserId != userId)
                throw new ForbiddenException("Vous n'êtes pas propriétaire de ce coffre.");

            // Vérifier que le nouveau nom n'est pas vide
            if (string.IsNullOrWhiteSpace(newName))
                throw new ValidationException(new Dictionary<string, string[]>
                {
                    { "Name", new[] { "Le nom du coffre ne peut pas être vide." } }
                });

            // Mettre à jour le nom
            vault.Name = newName;

            // Si un nouveau mot de passe est fourni, le mettre à jour également
            if (!string.IsNullOrWhiteSpace(newDerivedPassword) && !string.IsNullOrWhiteSpace(passwordSalt))
            {            
                // Concaténation mot de passe + sel client
                String passwordToHash = $"{newDerivedPassword}{passwordSalt}";

                // Hash BCrypt
                String bcryptHash = BCrypt.Net.BCrypt.HashPassword(passwordToHash);

                vault.HashPassword = bcryptHash;
                vault.PasswordSalt = passwordSalt;
            }

            await _vaultRepository.UpdateAsync(vault);

            // Retourner le coffre mis à jour
            return new GetVaultResponse
            {
                Id = vault.Id,
                Name = vault.Name,
                PasswordSalt = vault.PasswordSalt,
                CreatedAt = vault.CreatedAt,
                User = new VaultUserResponse
                {
                    Id = vault.UserId
                }
            };
        }
    }
}
