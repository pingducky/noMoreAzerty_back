namespace noMoreAzerty_back.Models
{
    public class Vault
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string HashPassword { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign Key
        public string UserId { get; set; }
        public User User { get; set; }

        // Navigation
        public ICollection<VaultEntry> VaultEntries { get; set; }
        public ICollection<VaultEntryHistory> VaultEntryHistories { get; set; }
        public ICollection<Share> Shares { get; set; }
    }
}
