using noMoreAzerty_back.Exceptions;
using noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Interfaces.noMoreAzerty_back.Interfaces;
using noMoreAzerty_back.Models.Enums;
using noMoreAzerty_dto.DTOs.Response;
using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.UseCases.History
{
    public class GetUserVaultEntryHistoryUseCase
    {
        private readonly IVaultEntryHistoryRepository _historyRepository;
        private readonly IUserRepository _userRepository;

        public GetUserVaultEntryHistoryUseCase(
            IVaultEntryHistoryRepository historyRepository,
            IUserRepository userRepository)
        {
            _historyRepository = historyRepository;
            _userRepository = userRepository;
        }

        public async Task<PagedResultResponse<VaultEntryHistoryItemResponse>> ExecuteAsync(
            Guid userId,
            VaultEntryAction[]? actions,
            int page,
            int pageSize)
        {
            if (page <= 0) page = 1;
            if (pageSize <= 0 || pageSize > 200) pageSize = 50;

            // Optionnel : vérifier que l'utilisateur existe
            var user = await _userRepository.GetByIdAsync(userId);
            if (user is null)
            {
                throw new NotFoundException("User not found");
            }

            var (items, totalCount) = await _historyRepository
                .GetByUserWithFiltersAsync(userId, actions, page, pageSize);

            var mapped = items.Select(h => new VaultEntryHistoryItemResponse
            {
                Id = h.Id,
                Action = h.Action.ToString(),
                CreatedAt = h.CreatedAt,
                VaultId = h.VaultId,
                EntryId = h.EntryId
            });

            return new PagedResultResponse<VaultEntryHistoryItemResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = mapped.ToList()
            };
        }
    }

    public class PagedResultResponse<T>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    }
}