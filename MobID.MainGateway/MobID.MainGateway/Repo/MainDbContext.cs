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
        public DbSet<UserAccess> UserAccesses { get; set; }
        public DbSet<OrganizationAccessShare> OrganizationAccessShares { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
                .HasOne(s => s.QrCode)
                .WithMany()
                .HasForeignKey(s => s.QrCodeId)
                .IsRequired(false);
            modelBuilder.Entity<Scan>()
               .HasOne(s => s.QrCode)
               .WithMany(q => q.Scans)                
               .HasForeignKey(s => s.QrCodeId)
               .IsRequired()                          
               .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<QrCode>()
                .HasQueryFilter(q => q.DeletedAt == null);
            modelBuilder.Entity<Scan>()
              .HasQueryFilter(s => s.DeletedAt == null);

            modelBuilder.Entity<RefreshToken>()
                .HasQueryFilter(rt => rt.User.DeletedAt == null);

            modelBuilder.Entity<UserAccess>(ent =>
            {
                ent.HasOne(ua => ua.User)
                        .WithMany(u => u.UserAccesses)
                        .HasForeignKey(ua => ua.UserId);
                
                ent.HasOne(ua => ua.Access)
                        .WithMany(a => a.UserAccesses)
                        .HasForeignKey(ua => ua.AccessId);
                
                ent.HasOne(ua => ua.GrantedByUser)
                        .WithMany()              // nu punem back-ref la granted-by
                        .HasForeignKey(ua => ua.GrantedByUserId);
                
                ent.Property(ua => ua.GrantType)
                        .HasConversion<string>();  // stocăm enum-ul ca text
                   });

            modelBuilder.Entity<OrganizationAccessShare>()
                .HasOne(s => s.SourceOrganization)
                .WithMany() // nu va exista colecția inversă pe Organization
                .HasForeignKey(s => s.SourceOrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizationAccessShare>()
                .HasOne(s => s.TargetOrganization)
                .WithMany(o => o.OrganizationAccessShares) // trebuie adăugat ICollection în Organization
                .HasForeignKey(s => s.TargetOrganizationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrganizationAccessShare>()
                .HasOne(s => s.Access)
                .WithMany(a => a.OrganizationAccessShares) // adaugă colecția în Access
                .HasForeignKey(s => s.AccessId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrganizationAccessShare>()
                .HasOne(s => s.Creator)
                .WithMany() // nu avem colecția inversă
                .HasForeignKey(s => s.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Soft-delete filter
            modelBuilder.Entity<OrganizationAccessShare>()
                .HasQueryFilter(s => s.DeletedAt == null);

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
