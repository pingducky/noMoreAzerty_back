using noMoreAzerty_back.Models.Enums;

namespace noMoreAzerty_back.Models
{
    public class VaultEntryHistory
    {
        public Guid Id { get; set; }

        public VaultEntryAction Action { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public Guid VaultId { get; set; }
        public Guid EntryId { get; set; }


        // Foreign Keys (navigation properties conservées pour EF)
        public Vault? Vault { get; set; }
        public User? User { get; set; }
        public VaultEntry? VaultEntry { get; set; }
    }
}
