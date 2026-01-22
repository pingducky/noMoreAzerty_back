using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Repositories;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class ShareVaultUseCase
    {
        private readonly IVaultRepository _vaultRepository;
        private readonly IUserRepository _userRepository;

        public ShareVaultUseCase(IVaultRepository vaultRepository, IUserRepository userRepository)
        {
            _vaultRepository = vaultRepository;
            _userRepository = userRepository;
        }

        public async Task<bool> ExecuteAsync(Guid vaultId, Guid ownerId, string userName)
        {
            // Vérifier que le coffre existe
            var vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                throw new NotFoundException("Vault not found.");

            // Vérifier que l'utilisateur actuel est bien le propriétaire du coffre
            if (vault.UserId != ownerId)
                throw new ForbiddenException("You are not the owner of this vault.");

            // Vérifier que l'utilisateur cible existe (par son nom)
            var targetUser = await _userRepository.GetByNameAsync(userName);
            if (targetUser == null)
                throw new NotFoundException($"User '{userName}' not found.");

            // Vérifier si le coffre est déjà partagé avec cet utilisateur
            if (await _vaultRepository.IsVaultSharedWithUserAsync(vaultId, targetUser.Id))
                throw new ForbiddenException("Vault is already shared with this user.");

            // Partager le coffre
            await _vaultRepository.ShareVaultAsync(vaultId, targetUser.Id);

            return true;
        }
    }
}
