using Microsoft.Graph;
using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Models;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_back.Services;
using noMoreAzerty_dto.DTOs.Response;
using static noMoreAzerty_back.Controllers.VaultEntryController;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class ValidateVaultAccessUseCase
    {
        private readonly IVaultRepository _vaultRepository;
        private readonly IVaultEntryRepository _vaultEntryRepository;

        public ValidateVaultAccessUseCase(IVaultRepository vaultRepository, IVaultEntryRepository vaultEntryRepository)
        {
            _vaultRepository = vaultRepository;
            _vaultEntryRepository = vaultEntryRepository;
        }

        public async Task<bool> ExecuteAsync(
            Guid vaultId,
            Guid userId,
            string password,
            string userIp)
        {
            // Récupération du coffre
            var vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                throw new NotFoundException("Vault not found.");

            // Vérification que l'utilisateur a accès au coffre
            if (!await _vaultRepository.UserHasAccessToVaultAsync(vaultId, userId))
                throw new ForbiddenException("User does not have access to this vault.");

            // Concaténation mot de passe + sel client
            var passwordToHash = $"{password}{vault.PasswordSalt}";

            // Comparaison sécurisée avec le hash du coffre
            if (!BCrypt.Net.BCrypt.Verify(passwordToHash, vault.HashPassword))
                throw new ForbiddenException("Incorrect password.");

            // Créer/mettre à jour la session après validation du mot de passe
            var sessionManager = VaultSessionManager.Instance;
            sessionManager.UpdateSession(userId, vaultId, userIp);

            return true;
        }
    }
}
