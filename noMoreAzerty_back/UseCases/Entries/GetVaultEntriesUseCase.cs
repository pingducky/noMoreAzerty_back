using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Models;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_dto.DTOs.Response;

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

        public async Task<IReadOnlyList<GetVaultEntriesResponse>> ExecuteAsync(Guid vaultId, Guid userId, string password)
        {
            // Récupération du coffre
            var vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                throw new NotFoundException("Vault not found.");

            // Vérification que l’utilisateur a accès au coffre
            if (!await _vaultRepository.UserHasAccessToVaultAsync(vaultId, userId))
                throw new ForbiddenException("User does not have access to this vault.");

            // Re-hash du mot de passe reçu
            var salt = Convert.FromBase64String(vault.PasswordSalt!);
            var computedHash = PasswordHasher.HashPassword(password, salt);

            // Concaténation mot de passe + sel client
            var passwordToHash = $"{password}{vault.PasswordSalt}";

            // Comparaison sécurisée avec le hash du coffre
            if (!BCrypt.Net.BCrypt.Verify(passwordToHash, vault.HashPassword))
                throw new ForbiddenException("Incorrect password.");

            List<VaultEntry> vaultEntries = await _vaultEntryRepository.GetEntriesByVaultAsync(vaultId);

            // Récupération des entrées chiffrées
            return vaultEntries.Select(ve => new GetVaultEntriesResponse
            {
                Id = ve.Id,
                CipherTitle = ve.CipherTitle,
                TitleIV = ve.TitleIV,
                TitleTag = ve.TitleTag,
                CipherUsername = ve.CipherUsername,
                UsernameIV = ve.UsernameIV,
                UsernameTag = ve.UsernameTag,
                CipherPassword = ve.CipherPassword,
                PasswordIV = ve.PasswordIV,
                PasswordTag = ve.PasswordTag,
                CipherUrl = ve.CipherUrl,
                UrlIV = ve.UrlIV,
                UrlTag = ve.UrlTag,
                CipherCommentary = ve.CipherCommentary,
                ComentaryIV = ve.ComentaryIV,
                ComentaryTag = ve.ComentaryTag,
                CreatedAt = ve.CreatedAt,
                UpdatedAt = ve.UpdatedAt
            })
            .ToList();
        }
    }
}
