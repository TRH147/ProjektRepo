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
        public DbSet<Admin> Admins { get; set; }

        public DbSet<Category> Categories { get; set; }
        public DbSet<ForumThread> ForumThreads { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ThreadTag> ThreadTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<UserDetails>().ToTable("userdetails");
            modelBuilder.Entity<LoginCode>().ToTable("logincodes");
            modelBuilder.Entity<PasswordResetCode>().ToTable("passwordresetcodes");
            modelBuilder.Entity<UserStats>().ToTable("userstats");
            modelBuilder.Entity<FriendRequest>().ToTable("friendrequests");
            modelBuilder.Entity<Admin>().ToTable("admins");
            modelBuilder.Entity<Category>().ToTable("categories");
            modelBuilder.Entity<ForumThread>().ToTable("forumthreads");
            modelBuilder.Entity<Post>().ToTable("posts");
            modelBuilder.Entity<Tag>().ToTable("tags");
            modelBuilder.Entity<ThreadTag>().ToTable("threadtags");

            modelBuilder.Entity<User>()
                .HasOne(u => u.UserStats)
                .WithOne(s => s.User)
                .HasForeignKey<UserStats>(s => s.Id);

            modelBuilder.Entity<ThreadTag>()
                .HasKey(tt => new { tt.ThreadId, tt.TagId });

            modelBuilder.Entity<ForumThread>()
                .HasOne(t => t.Author)
                .WithMany()
                .HasForeignKey(t => t.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ForumThread>()
                .HasOne(t => t.Category)
                .WithMany(c => c.Threads)
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Author)
                .WithMany()
                .HasForeignKey(p => p.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.Thread)
                .WithMany(t => t.Posts)
                .HasForeignKey(p => p.ThreadId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Post>()
                .HasOne(p => p.ParentPost)
                .WithMany(p => p.Replies)
                .HasForeignKey(p => p.ParentPostId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Credential)
                .WithOne(ud => ud.User)
                .HasForeignKey<UserDetails>(ud => ud.UserId);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Sender)
                .WithMany()
                .HasForeignKey(fr => fr.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FriendRequest>()
                .HasOne(fr => fr.Receiver)
                .WithMany()
                .HasForeignKey(fr => fr.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ForumThread>()
                .HasIndex(t => t.CreatedAt);

            modelBuilder.Entity<Post>()
                .HasIndex(p => p.ThreadId);

            modelBuilder.Entity<Post>()
                .HasIndex(p => p.CreatedAt);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<UserDetails>()
                .HasIndex(ud => ud.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .HasColumnType("varchar(50)");

            modelBuilder.Entity<User>()
                .Property(u => u.ProfileImages)
                .HasColumnType("varchar(500)");

            modelBuilder.Entity<UserDetails>()
                .Property(ud => ud.Email)
                .HasColumnType("varchar(100)");

            modelBuilder.Entity<UserDetails>()
                .Property(ud => ud.PasswordHash)
                .HasColumnType("varchar(255)");

            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .HasColumnType("varchar(100)");

            modelBuilder.Entity<Category>()
                .Property(c => c.Description)
                .HasColumnType("varchar(500)");

            modelBuilder.Entity<Category>()
                .Property(c => c.Icon)
                .HasColumnType("varchar(100)");

            modelBuilder.Entity<ForumThread>()
                .Property(t => t.Title)
                .HasColumnType("varchar(200)");

            modelBuilder.Entity<ForumThread>()
                .Property(t => t.Content)
                .HasColumnType("text");

            modelBuilder.Entity<Post>()
                .Property(p => p.Content)
                .HasColumnType("text");

            modelBuilder.Entity<Tag>()
                .Property(t => t.Name)
                .HasColumnType("varchar(50)");

            modelBuilder.Entity<Tag>()
                .Property(t => t.Color)
                .HasColumnType("varchar(20)");

            modelBuilder.Entity<Admin>()
                .Property(a => a.Username)
                .HasColumnType("varchar(50)");

            modelBuilder.Entity<Admin>()
                .Property(a => a.Password)
                .HasColumnType("varchar(255)");

            modelBuilder.Entity<LoginCode>()
                .Property(lc => lc.Email)
                .HasColumnType("varchar(100)");

            modelBuilder.Entity<LoginCode>()
                .Property(lc => lc.Code)
                .HasColumnType("varchar(10)");

            modelBuilder.Entity<PasswordResetCode>()
                .Property(prc => prc.Email)
                .HasColumnType("varchar(100)");

            modelBuilder.Entity<PasswordResetCode>()
                .Property(prc => prc.Code)
                .HasColumnType("varchar(10)");

            modelBuilder.Entity<FriendRequest>()
                .Property(fr => fr.Status)
                .HasColumnType("varchar(20)");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "System",
                    ProfileImages = "/uploads/profile-images/simple-outline-user-configuration-setting-600nw-2636195015.webp",
                    LastActive = DateTime.UtcNow
                }
            );

            modelBuilder.Entity<UserDetails>().HasData(
                new UserDetails
                {
                    Id = 1,
                    UserId = 1,
                    Email = "system@forum.com",
                    PasswordHash = "",
                    CreatedAt = DateTime.UtcNow.AddDays(-7)
                }
            );

            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Username = "admin",
                    Password = "admin123"
                }
            );

            modelBuilder.Entity<Category>().HasData(
                new Category
                {
                    Id = 1,
                    Name = "Általános csevegés",
                    Description = "Minden, ami nem fér bele a többi kategóriába",
                    Order = 1,
                    CreatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Id = 2,
                    Name = "Műszaki segítség",
                    Description = "Segítségnyújtás technikai akadályok elhárításához",
                    Order = 2,
                    CreatedAt = DateTime.UtcNow
                },
                new Category
                {
                    Id = 3,
                    Name = "Közlemények",
                    Description = "Fontos hírek és hivatalos információk",
                    Order = 0,
                    Icon = "bell",
                    CreatedAt = DateTime.UtcNow
                }
            );

            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name = "Kérdés", Color = "#3498db" },
                new Tag { Id = 2, Name = "Segítség", Color = "#e74c3c" },
                new Tag { Id = 3, Name = "Megbeszélés", Color = "#2ecc71" },
                new Tag { Id = 4, Name = "Hiba", Color = "#e67e22" },
                new Tag { Id = 5, Name = "Fejlesztés", Color = "#9b59b6" },
                new Tag { Id = 6, Name = "Leírás", Color = "#1abc9c" },
                new Tag { Id = 7, Name = "Hírek", Color = "#f1c40f" },
                new Tag { Id = 8, Name = "Fontos", Color = "#e74c3c" }
            );

            modelBuilder.Entity<ForumThread>().HasData(
                new ForumThread
                {
                    Id = 1,
                    Title = "Üdvözlünk a Fórumon! 🎉",
                    Content = "Üdvözlünk mindenkit a fórumunkon! Ez egy olyan hely, ahol különböző témákat vitathattok meg, kérdéseket tehettek fel, és megoszthatjátok a tudásotokat. Kérjük, posztolás előtt olvassátok el a szabályzatot! Mindenkinek kellemes időtöltést kívánunk!",
                    AuthorId = 1,
                    CategoryId = 3,
                    CreatedAt = DateTime.UtcNow.AddDays(-1),
                    IsLocked = false,
                    IsPinned = true,
                    ViewCount = 0,
                }
            );

            modelBuilder.Entity<ThreadTag>().HasData(
                new ThreadTag { ThreadId = 1, TagId = 7 },
                new ThreadTag { ThreadId = 1, TagId = 8 },
                new ThreadTag { ThreadId = 1, TagId = 3 }
            );
        }
    }
}