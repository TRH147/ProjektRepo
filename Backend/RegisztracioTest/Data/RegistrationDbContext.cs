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

        public DbSet<UserStats> UserStats => Set<UserStats>();

        public DbSet<FriendRequest> FriendRequests { get; set; } = null!;

        // ADD Admin table
        public DbSet<Admin> Admins { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User ↔ UserStats one-to-one relation
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserStats)
                .WithOne(s => s.User)
                .HasForeignKey<UserStats>(s => s.Id);

            // Seed default admin
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123" // Teszteléshez, élesben hash-elni kell
                }
            );
        }
    }
}
