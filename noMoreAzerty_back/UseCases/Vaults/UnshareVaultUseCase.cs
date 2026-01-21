using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Repositories;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class UnshareVaultUseCase
    {
        private readonly IVaultRepository _vaultRepository;

        public UnshareVaultUseCase(IVaultRepository vaultRepository)
        {
            _vaultRepository = vaultRepository;
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

            // Vérifier si le coffre est bien partagé avec cet utilisateur
            if (!await _vaultRepository.IsVaultSharedWithUserAsync(vaultId, targetUserId))
                return false; // Pas partagé, rien à faire

            // Supprimer le partage
            await _vaultRepository.UnshareVaultAsync(vaultId, targetUserId);

            return true;
        }
    }
}
