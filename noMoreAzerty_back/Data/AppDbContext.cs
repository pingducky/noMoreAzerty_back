using Microsoft.EntityFrameworkCore;
using noMoreAzerty_back.Models;

namespace MyApiProject.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Vault> Vaults { get; set; }
        public DbSet<VaultEntry> VaultEntries { get; set; }
        public DbSet<VaultEntryHistory> VaultEntryHistories { get; set; }
        public DbSet<Share> Shares { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // === TABLE Share : clé composite ===
            modelBuilder.Entity<Share>()
                .HasKey(s => new { s.UserId, s.VaultId });

            // User ↔ Vault (1:N)
            modelBuilder.Entity<Vault>()
                .HasOne(v => v.User)
                .WithMany(u => u.Vaults)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Vault ↔ VaultEntry (1:N)
            modelBuilder.Entity<VaultEntry>()
                .HasOne(e => e.Vault)
                .WithMany(v => v.VaultEntries)
                .HasForeignKey(e => e.VaultId)
                .OnDelete(DeleteBehavior.Cascade);

            // Vault ↔ VaultEntryHistory (1:N)
            modelBuilder.Entity<VaultEntryHistory>()
                .HasOne(h => h.Vault)
                .WithMany(v => v.VaultEntryHistories)
                .HasForeignKey(h => h.VaultId)
                .OnDelete(DeleteBehavior.Restrict);

            // User ↔ VaultEntryHistory (1:N)
            modelBuilder.Entity<VaultEntryHistory>()
                .HasOne(h => h.User)
                .WithMany(u => u.VaultEntryHistories)
                .HasForeignKey(h => h.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // VaultEntry ↔ VaultEntryHistory (1:N)
            modelBuilder.Entity<VaultEntryHistory>()
                .HasOne(h => h.VaultEntry)
                .WithMany(e => e.VaultEntryHistories)
                .HasForeignKey(h => h.VaultEntryId)
                .OnDelete(DeleteBehavior.Cascade);

            // User ↔ Share (N:N via Share table)
            modelBuilder.Entity<Share>()
                .HasOne(s => s.User)
                .WithMany(u => u.Shares)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Restrict); // supprime la cascade

            // Vault ↔ Share (N:N via Share table)
            modelBuilder.Entity<Share>()
                .HasOne(s => s.Vault)
                .WithMany(v => v.Shares)
                .HasForeignKey(s => s.VaultId)
                .OnDelete(DeleteBehavior.Restrict); // supprime la cascade

            // === Configurations optionnelles ===
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Vault>().ToTable("Vault");
            modelBuilder.Entity<VaultEntry>().ToTable("VaultEntry");
            modelBuilder.Entity<VaultEntryHistory>().ToTable("VaultEntryHistory");
            modelBuilder.Entity<Share>().ToTable("Share");

            base.OnModelCreating(modelBuilder);
        }
    }
}
