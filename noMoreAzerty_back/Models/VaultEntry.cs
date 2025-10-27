namespace noMoreAzerty_back.Models
{
    public class VaultEntry
    {
        public string Id { get; set; }
        public string CipherTitle { get; set; }
        public string TitleIV { get; set; }
        public string TitleTag { get; set; }
        public string CipherUsername { get; set; }
        public string UsernameIV { get; set; }
        public string UsernameTag { get; set; }
        public string CipherPassword { get; set; }
        public string PasswordIV { get; set; }
        public string PasswordTag { get; set; }
        public string CipherUrl { get; set; }
        public string UrlIV { get; set; }
        public string UrlTag { get; set; }
        public string CipherCommentary { get; set; }
        public string ComentaryIV { get; set; }
        public string ComentaryTag { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }

        // Foreign Key
        public string VaultId { get; set; }
        public Vault Vault { get; set; }

        // Navigation
        public ICollection<VaultEntryHistory> VaultEntryHistories { get; set; }
    }
}
    