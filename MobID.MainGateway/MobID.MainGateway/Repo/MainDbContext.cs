using Microsoft.EntityFrameworkCore;
using MobID.MainGateway.Models.Entities;
using System.Linq.Expressions;
using System;

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
        public DbSet<UserAccess> UserAccesses { get; set; }
        public DbSet<OrganizationAccessShare> OrganizationAccessShares { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // ─── UserRole: cheie compusă, restrict delete ─────────────────────
            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.UserId, ur.RoleId });
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            // ─── OrganizationUser: restrict delete ───────────────────────────
            modelBuilder.Entity<OrganizationUser>()
                .HasOne(ou => ou.User)
                .WithMany(u => u.OrganizationUsers)
                .HasForeignKey(ou => ou.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<OrganizationUser>()
                .HasOne(ou => ou.Organization)
                .WithMany(o => o.OrganizationUsers)
                .HasForeignKey(ou => ou.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            // ─── Access ↔ Organization & CreatedByUser ────────────────────────
            modelBuilder.Entity<Access>()
                .HasOne(a => a.Organization)
                .WithMany()
                .HasForeignKey(a => a.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Access>()
                .HasOne(a => a.CreatedByUser)
                .WithMany()
                .HasForeignKey(a => a.CreatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ─── QrCode ↔ Access: restrict delete ────────────────────────────
            modelBuilder.Entity<QrCode>()
                .HasOne(q => q.Access)
                .WithMany(a => a.QrCodes)
                .HasForeignKey(q => q.AccessId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<QrCode>()
                .HasQueryFilter(q => q.DeletedAt == null);

            // ─── Scan ↔ QrCode: cascade delete on QR removal, restrict otherwise ─
            modelBuilder.Entity<Scan>()
                .HasOne(s => s.QrCode)
                .WithMany(q => q.Scans)
                .HasForeignKey(s => s.QrCodeId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Scan>()
                .HasQueryFilter(s => s.DeletedAt == null);

            // ─── RefreshToken ↔ User: restrict delete ─────────────────────────
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithOne(u => u.RefreshToken)
                .HasForeignKey<RefreshToken>(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ─── UserAccess: restrict delete ─────────────────────────────────
            modelBuilder.Entity<UserAccess>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAccesses)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<UserAccess>()
                .HasOne(ua => ua.Access)
                .WithMany(a => a.UserAccesses)
                .HasForeignKey(ua => ua.AccessId)
                .OnDelete(DeleteBehavior.Restrict);

            // ─── OrganizationAccessShare ────────────────────────────────────
            modelBuilder.Entity<OrganizationAccessShare>()
                .HasOne(s => s.SourceOrganization)
                .WithMany()
                .HasForeignKey(s => s.SourceOrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizationAccessShare>()
                .HasOne(s => s.TargetOrganization)
                .WithMany(o => o.OrganizationAccessShares)
                .HasForeignKey(s => s.TargetOrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizationAccessShare>()
                .HasOne(s => s.Access)
                .WithMany(a => a.OrganizationAccessShares)
                .HasForeignKey(s => s.AccessId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizationAccessShare>()
                .HasOne(s => s.Creator)
                .WithMany()
                .HasForeignKey(s => s.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizationAccessShare>()
                .HasQueryFilter(s => s.DeletedAt == null);

            // ─── Seed AccessType ─────────────────────────────────────────────
            modelBuilder.Entity<AccessType>().HasData(
                new AccessType
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0001"),
                    Name = "LimitedUse",
                    Description = "Acces cu limită totală de utilizări",
                    IsLimitedUse = true,
                    IsSubscription = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new AccessType
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0002"),
                    Name = "Subscription",
                    Description = "Acces în abonament pe perioade",
                    IsLimitedUse = false,
                    IsSubscription = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new AccessType
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaa0003"),
                    Name = "IdentityConfirm",
                    Description = "Confirmare unică de identitate per utilizator",
                    IsLimitedUse = false,
                    IsSubscription = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            // ─── Seed Role ───────────────────────────────────────────────────
            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Name = "Admin",
                    Description = "Administrator role",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Role
                {
                    Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                    Name = "OrgUser",
                    Description = "Organization user role",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new Role
                {
                    Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    Name = "SimpleUser",
                    Description = "Simple user role",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );

            // ─── Seed Users & UserRole ───────────────────────────────────────
            var pwd = BCrypt.Net.BCrypt.HashPassword("Qwerty1!");
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    Email = "admin@example.com",
                    Username = "admin",
                    PasswordHash = pwd,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                },
                new User
                {
                    Id = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                    Email = "user@example.com",
                    Username = "user",
                    PasswordHash = pwd,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                }
            );
            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = Guid.Parse("11111111-2222-3333-4444-555555555555"),
                    UserId = Guid.Parse("dddddddd-dddd-dddd-dddd-dddddddddddd"),
                    RoleId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    IsActive = true
                },
                new UserRole
                {
                    Id = Guid.Parse("66666666-7777-8888-9999-aaaaaaaaaaaa"),
                    UserId = Guid.Parse("eeeeeeee-eeee-eeee-eeee-eeeeeeeeeeee"),
                    RoleId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                    IsActive = true
                }
            );
        }

        // Utility pentru filtre soft-delete (dacă vrei să-l reactivezi)
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
