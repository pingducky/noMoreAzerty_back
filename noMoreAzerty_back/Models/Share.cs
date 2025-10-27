namespace noMoreAzerty_back.Models
{
    public class Share
    {
        public string UserId { get; set; }
        public User User { get; set; }

        public string VaultId { get; set; }
        public Vault Vault { get; set; }

        public DateTime AddedAt { get; set; }
    }
}
