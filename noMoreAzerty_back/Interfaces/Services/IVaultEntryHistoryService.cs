using noMoreAzerty_back.Models;
using noMoreAzerty_back.Models.Enums;
using System;

namespace noMoreAzerty_back.Interfaces.Services
{
    public interface IVaultEntryHistoryService
    {
        Task LogEntryCreatedAsync(
            VaultEntryAction action,
            Guid userId,
            Guid vaultId,
            VaultEntry entry
        );
    }
}
