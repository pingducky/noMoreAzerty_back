namespace noMoreAzerty_back.Models
{
    public class Vault
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string HashPassword { get; set; } = null!;
        public string PasswordSalt { get; set; } = null!;
        public DateTime CreatedAt { get; set; }

        // Foreign Key
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<VaultEntry> VaultEntries { get; set; } = new HashSet<VaultEntry>();
        public ICollection<VaultEntryHistory> VaultEntryHistories { get; set; } = new HashSet<VaultEntryHistory>();
        public ICollection<Share> Shares { get; set; } = new HashSet<Share>();
    }
}
