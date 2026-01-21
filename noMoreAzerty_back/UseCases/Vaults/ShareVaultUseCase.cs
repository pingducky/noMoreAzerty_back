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

        public async Task<bool> ExecuteAsync(Guid vaultId, Guid ownerId, Guid targetUserId)
        {
            // Vérifier que le coffre existe
            var vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                throw new NotFoundException("Le coffre n'existe pas.");

            // Vérifier que l'utilisateur actuel est bien le propriétaire du coffre
            if (vault.UserId != ownerId)
                throw new ForbiddenException("Vous n'êtes pas propriétaire de ce coffre.");

            // Vérifier que l'utilisateur cible existe
            var targetUser = await _userRepository.GetByIdAsync(targetUserId);
            if (targetUser == null)
                throw new NotFoundException("L'utilisateur cible n'existe pas.");

            // Vérifier si le coffre est déjà partagé avec cet utilisateur
            if (await _vaultRepository.IsVaultSharedWithUserAsync(vaultId, targetUserId))
                return false; // Déjà partagé

            // Partager le coffre
            await _vaultRepository.ShareVaultAsync(vaultId, targetUserId);

            return true;
        }
    }
}
