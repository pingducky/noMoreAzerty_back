using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Repositories;
using noMoreAzerty_dto.DTOs.Response;

namespace noMoreAzerty_back.UseCases.Vaults
{
    public class GetVaultUsersUseCase
    {
        private readonly IVaultRepository _vaultRepository;
        private readonly IUserRepository _userRepository;

        public GetVaultUsersUseCase(IVaultRepository vaultRepository, IUserRepository userRepository)
        {
            _vaultRepository = vaultRepository;
            _userRepository = userRepository;
        }

        public async Task<VaultUsersResponse> ExecuteAsync(Guid vaultId, Guid requestingUserId)
        {
            // Vérifier que le coffre existe
            var vault = await _vaultRepository.GetByIdAsync(vaultId);
            if (vault == null)
                throw new NotFoundException($"Le coffre {vaultId} n'existe pas.");

            // Vérifier que l'utilisateur a accès au coffre
            bool isOwner = vault.UserId == requestingUserId;
            bool hasAccess = isOwner || await _vaultRepository.UserHasAccessToVaultAsync(vaultId, requestingUserId);

            if (!hasAccess)
                throw new ForbiddenException("Vous n'avez pas accès à ce coffre.");

            // Récupérer tous les utilisateurs ayant accès
            var usersWithAccess = await _userRepository.GetVaultUsersAsync(vaultId);

            return new VaultUsersResponse
            {
                VaultId = vaultId,
                VaultName = vault.Name,
                OwnerId = vault.UserId,
                Users = usersWithAccess.Select(u => new VaultUserDto
                {
                    UserId = u.User.Id,
                    Name = u.User.Name,
                    IsOwner = u.IsOwner,
                    SharedAt = u.SharedAt
                }).ToList()
            };
        }
    }
}