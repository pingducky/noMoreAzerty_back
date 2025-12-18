using noMoreAzerty_back.Models;
using noMoreAzerty_back.Repositories;

namespace noMoreAzerty_back.UseCases.Entries
{
    public class GetVaultEntriesUseCase
    {
        private readonly IVaultRepository _vaultRepository;
        private readonly IVaultEntryRepository _vaultEntryRepository;

        public GetVaultEntriesUseCase(IVaultRepository vaultRepository, IVaultEntryRepository vaultEntryRepository)
        {
            _vaultRepository = vaultRepository;
            _vaultEntryRepository = vaultEntryRepository;
        }

        public async Task<IReadOnlyList<VaultEntry>> ExecuteAsync(Guid vaultId, Guid userId, string password)
        {
            // Récupération du coffre
            var vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                throw new KeyNotFoundException("Vault not found.");

            // Vérification que l’utilisateur a accès au coffre
            if (!await _vaultRepository.UserHasAccessToVaultAsync(vaultId, userId))
                throw new UnauthorizedAccessException("User does not have access to this vault.");

            // Re-hash du mot de passe reçu
            var salt = Convert.FromBase64String(vault.PasswordSalt!);
            var computedHash = PasswordHasher.HashPassword(password, salt);

            // Concaténation mot de passe + sel client
            var passwordToHash = $"{password}{vault.PasswordSalt}";

            // Comparaison sécurisée avec le hash du coffre
            if (!BCrypt.Net.BCrypt.Verify(passwordToHash, vault.HashPassword))
                throw new UnauthorizedAccessException("Incorrect password.");

            // Récupération des entrées chiffrées
            return await _vaultEntryRepository.GetEntriesByVaultAsync(vaultId);
        }
    }
}
