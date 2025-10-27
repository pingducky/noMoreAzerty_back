using System;
using System.Collections.Generic;

namespace noMoreAzerty_back.Models
{
    public class User
    {
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }

        // Navigation
        public ICollection<Vault> Vaults { get; set; }
        public ICollection<VaultEntryHistory> VaultEntryHistories { get; set; }
        public ICollection<Share> Shares { get; set; }
    }
}
