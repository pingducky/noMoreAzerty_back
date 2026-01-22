using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Interfaces.Services;
using noMoreAzerty_back.Models;
using noMoreAzerty_back.Models.Enums;
using noMoreAzerty_back.Repositories;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class DeleteVaultUseCase
    {
        private readonly IVaultRepository _vaultRepository;


        public DeleteVaultUseCase(IVaultRepository vaultRepository)
        {
            _vaultRepository = vaultRepository;
        }

        public async Task ExecuteAsync(Guid vaultId, Guid userId)
        {
            // Vérifier que le coffre existe
            Vault? vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                throw new NotFoundException("Le coffre n'existe pas.");

            // Vérifier que l'utilisateur est bien le propriétaire
            if (vault.UserId != userId)
                throw new ForbiddenException("Vous n'êtes pas propriétaire de ce coffre.");

            // Supprimer le coffre (cascade delete pour les entrées et partages)
            await _vaultRepository.DeleteAsync(vaultId);

        }
    }
}
