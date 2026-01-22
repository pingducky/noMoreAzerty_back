using Microsoft.EntityFrameworkCore;
using noMoreAzerty_back.Models;

namespace noMoreAzerty_back.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Vault> Vaults { get; set; }
        public DbSet<VaultEntry> VaultEntries { get; set; }
        public DbSet<VaultEntryHistory> VaultEntryHistory { get; set; }
        public DbSet<Share> Shares { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Share>().HasKey(s => new { s.UserId, s.VaultId });
            modelBuilder.Entity<Vault>()
                .HasOne(v => v.User)
                .WithMany(u => u.Vaults)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<VaultEntry>()
                .HasOne(e => e.Vault)
                .WithMany(v => v.VaultEntries)
                .HasForeignKey(e => e.VaultId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<VaultEntryHistory>()
                .HasOne(h => h.Vault)
                .WithMany(v => v.VaultEntryHistories)
                .HasForeignKey(h => h.VaultId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VaultEntryHistory>()
                .HasOne(h => h.User)
                .WithMany(u => u.VaultEntryHistories)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<VaultEntryHistory>()
                .HasOne(h => h.VaultEntry)
                .WithMany(e => e.VaultEntryHistories)
                .HasForeignKey(h => h.EntryId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Share>()
                .HasOne(s => s.User)
                .WithMany(u => u.Shares)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Share>()
                .HasOne(s => s.Vault)
                .WithMany(v => v.Shares)
                .HasForeignKey(s => s.VaultId)
                .OnDelete(DeleteBehavior.Restrict);

            // Config tables
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Vault>().ToTable("Vault");
            modelBuilder.Entity<VaultEntry>().ToTable("VaultEntry");
            modelBuilder.Entity<VaultEntryHistory>().ToTable("VaultEntryHistory");
            modelBuilder.Entity<Share>().ToTable("Share");

            // === SEEDING (avec valeurs fixes) ===
            var userId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
            var vaultId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
            var entry1Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
            var entry2Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd");
            var historyId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee");

            var createdAt = new DateTime(2024, 01, 01);

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = userId,
                    CreatedAt = createdAt,
                    LastLogin = createdAt,
                    IsActive = true
                }
            );

            modelBuilder.Entity<Vault>().HasData(
                new Vault
                {
                    Id = vaultId,
                    Name = "Vault personnel",
                    HashPassword = "hashed-password-demo",
                    PasswordSalt = "random-salt-demo",
                    CreatedAt = createdAt,
                    UserId = userId
                }
            );

            modelBuilder.Entity<VaultEntry>().HasData(
                new VaultEntry
                {
                    Id = entry1Id,
                    CipherTitle = "cipher_gmail_title",
                    TitleIV = "iv1",
                    TitleTag = "tag1",
                    CipherUsername = "cipher_gmail_user",
                    UsernameIV = "iv2",
                    UsernameTag = "tag2",
                    CipherPassword = "cipher_gmail_password",
                    PasswordIV = "iv3",
                    PasswordTag = "tag3",
                    CipherUrl = "cipher_gmail_url",
                    UrlIV = "iv4",
                    UrlTag = "tag4",
                    CipherCommentary = "cipher_comment",
                    ComentaryIV = "iv5",
                    ComentaryTag = "tag5",
                    CreatedAt = createdAt,
                    UpdatedAt = createdAt,
                    IsActive = true,
                    VaultId = vaultId
                },
                new VaultEntry
                {
                    Id = entry2Id,
                    CipherTitle = "cipher_github_title",
                    TitleIV = "iv6",
                    TitleTag = "tag6",
                    CipherUsername = "cipher_github_user",
                    UsernameIV = "iv7",
                    UsernameTag = "tag7",
                    CipherPassword = "cipher_github_password",
                    PasswordIV = "iv8",
                    PasswordTag = "tag8",
                    CipherUrl = "cipher_github_url",
                    UrlIV = "iv9",
                    UrlTag = "tag9",
                    CipherCommentary = "cipher_comment2",
                    ComentaryIV = "iv10",
                    ComentaryTag = "tag10",
                    CreatedAt = createdAt,
                    UpdatedAt = createdAt,
                    IsActive = true,
                    VaultId = vaultId
                }
            );

            modelBuilder.Entity<VaultEntryHistory>().HasData(
                new VaultEntryHistory
                {
                    Id = historyId,
                    Action = noMoreAzerty_back.Models.Enums.VaultEntryAction.Created,
                    CreatedAt = createdAt,
                    UserId = userId,
                    VaultId = vaultId,
                    EntryId = entry1Id
                }
            );


            modelBuilder.Entity<Share>().HasData(
                new Share
                {
                    UserId = userId,
                    VaultId = vaultId,
                    AddedAt = createdAt
                }
            );

            base.OnModelCreating(modelBuilder);
        }
    }
}
