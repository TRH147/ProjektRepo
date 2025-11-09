using Microsoft.EntityFrameworkCore;
using RegisztracioTest.Model;
using System;

namespace RegisztracioTest.Data
{
    public class RegistrationDbContext : DbContext
    {
        public RegistrationDbContext(DbContextOptions<RegistrationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();

        public DbSet<UserDetails> UserDetails { get; set; }

        public DbSet<LoginCode> LoginCodes { get; set; }

        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }

        // ADD THIS — user stats table
        public DbSet<UserStats> UserStats => Set<UserStats>();

        public DbSet<FriendRequest> FriendRequests { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User ↔ UserStats one-to-one relation
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserStats)
                .WithOne(s => s.User)
                .HasForeignKey<UserStats>(s => s.Id);
        }
    }
}
