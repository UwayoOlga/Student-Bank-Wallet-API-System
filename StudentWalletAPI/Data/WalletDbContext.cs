using Microsoft.EntityFrameworkCore;
using StudentWalletAPI.Models;

namespace StudentWalletAPI.Data
{
    public class WalletDbContext : DbContext
    {
        public WalletDbContext(DbContextOptions<WalletDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Student entity
            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.StudentId);
                entity.HasIndex(e => e.StudentId).IsUnique();
            });

            // Configure Wallet entity
            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.HasKey(e => e.WalletId);
                entity.HasIndex(e => e.WalletId).IsUnique();
                entity.Property(e => e.Balance).HasPrecision(18, 2);

                // One-to-One relationship: Student -> Wallet
                entity.HasOne(w => w.Student)
                    .WithOne(s => s.Wallet)
                    .HasForeignKey<Wallet>(w => w.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Transaction entity
            modelBuilder.Entity<Transaction>(entity =>
            {
                entity.HasKey(e => e.TransactionId);
                entity.HasIndex(e => e.TransactionId).IsUnique();
                entity.Property(e => e.Amount).HasPrecision(18, 2);

                // Many-to-One relationship: Transaction -> Wallet
                entity.HasOne(t => t.Wallet)
                    .WithMany(w => w.Transactions)
                    .HasForeignKey(t => t.WalletId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Optional relationship for transfers: Transaction -> ReceiverWallet
                entity.HasOne(t => t.ReceiverWallet)
                    .WithMany()
                    .HasForeignKey(t => t.ReceiverWalletId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Note: Seed data is handled in Program.cs to avoid conflicts
        }
    }
}