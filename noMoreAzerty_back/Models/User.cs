namespace noMoreAzerty_back.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }

        // Navigation
        public HashSet<Vault>? Vaults { get; set; }
        public HashSet<VaultEntryHistory>? VaultEntryHistories { get; set; }
        public HashSet<Share>? Shares { get; set; }
        public HashSet<UserRole>? Roles { get; set; } = new();
    }
}
