using Microsoft.EntityFrameworkCore;
using MobID.MainGateway.Models.Entities;
using System.Linq.Expressions;
using System;
using System.Linq;

namespace MobID.MainGateway.Repo
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }

        // DbSet-urile pentru entități
        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<OrganizationUser> OrganizationUsers { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<Access> Accesses { get; set; }
        public DbSet<AccessType> AccessTypes { get; set; }
        public DbSet<QrCode> QrCodes { get; set; }
        public DbSet<Scan> Scans { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurarea relațiilor
            modelBuilder.Entity<OrganizationUser>()
                .HasKey(ou => new { ou.OrganizationId, ou.UserId });
            modelBuilder.Entity<OrganizationUser>()
                .HasOne(ou => ou.Organization)
                .WithMany(o => o.OrganizationUsers)
                .HasForeignKey(ou => ou.OrganizationId);
            modelBuilder.Entity<OrganizationUser>()
                .HasOne(ou => ou.User)
                .WithMany(u => u.OrganizationUsers)
                .HasForeignKey(ou => ou.UserId);

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<Access>()
                .HasOne(a => a.Organization)
                .WithMany()
                .HasForeignKey(a => a.OrganizationId);
            modelBuilder.Entity<Access>()
                .HasOne(a => a.Creator)
                .WithMany()
                .HasForeignKey(a => a.CreatedBy);
            modelBuilder.Entity<QrCode>()
                .HasOne(qr => qr.Access)
                .WithMany(a => a.QrCodes)
                .HasForeignKey(qr => qr.AccessId);
            modelBuilder.Entity<Scan>()
                .HasOne(s => s.Access)
                .WithMany()
                .HasForeignKey(s => s.AccessId);
            modelBuilder.Entity<Scan>()
                .HasOne(s => s.QrCode)
                .WithMany()
                .HasForeignKey(s => s.QrCodeId)
                .IsRequired(false);
            modelBuilder.Entity<Scan>()
                .HasOne(s => s.ScannedBy)
                .WithMany()
                .HasForeignKey(s => s.ScannedById);
            modelBuilder.Entity<RefreshToken>()
                .HasQueryFilter(rt => rt.User.DeletedAt == null);

            // Soft Delete: Query Filter pentru DeletedAt
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.GetProperty("DeletedAt") != null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
                }
            }

            // Seed Data pentru AccessType
            modelBuilder.Entity<AccessType>().HasData(
                new AccessType
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "OneUse",
                    Description = "One time usage access",
                    IsLimitedUse = true,
                    IsSubscription = false,
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new AccessType
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "MultiUse",
                    Description = "Multiple usage access",
                    IsLimitedUse = true,
                    IsSubscription = false,
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new AccessType
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Subscription",
                    Description = "Subscription access",
                    IsLimitedUse = false,
                    IsSubscription = true,
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new AccessType
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "IdentityConfirm",
                    Description = "Identity confirmation access",
                    IsLimitedUse = false,
                    IsSubscription = false,
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed Data pentru Role
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Name = "Admin",
                    Description = "Administrator role",
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Role
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Name = "OrgUser",
                    Description = "Organization user role",
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Role
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    Name = "SimpleUser",
                    Description = "Simple user role",
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Qwerty1!");

            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    Email = "admin@example.com",
                    Username = "admin",
                    PasswordHash = passwordHash,
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new User
                {
                    Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                    Email = "user@example.com",
                    Username = "user",
                    PasswordHash = passwordHash,
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Seed Data pentru UserRole (Admin primește rol Admin; User primește rol SimpleUser)
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    UserId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    RoleId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    IsActive = true,
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new UserRole
                {
                    UserId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                    RoleId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    IsActive = true,
                    CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );
        }

        private static LambdaExpression ConvertFilterExpression(Type entityType)
        {
            var param = Expression.Parameter(entityType, "e");
            var property = Expression.Property(param, "DeletedAt");
            var nullValue = Expression.Constant(null, typeof(DateTime?));
            var condition = Expression.Equal(property, nullValue);
            return Expression.Lambda(condition, param);
        }
    }
}
