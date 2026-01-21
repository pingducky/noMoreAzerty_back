namespace noMoreAzerty_back.Models
{
    public class UserRole
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Role { get; set; } = "User"; // "Admin", "User"
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public User User { get; set; } = null!;
    }
}
