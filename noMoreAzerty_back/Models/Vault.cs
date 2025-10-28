namespace noMoreAzerty_back.Models
{
    public class Vault
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? HashPassword { get; set; }
        public string? PasswordSalt { get; set; }
        public DateTime CreatedAt { get; set; }

        // Foreign Key
        public Guid UserId { get; set; }
        public User? User { get; set; }

        // Navigation
        public HashSet<VaultEntry>? VaultEntries { get; set; }
        public HashSet<VaultEntryHistory>? VaultEntryHistories { get; set; }
        public HashSet<Share>? Shares { get; set; }
    }
}
