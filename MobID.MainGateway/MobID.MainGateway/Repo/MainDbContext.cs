using Microsoft.EntityFrameworkCore;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Enums;
using System.Linq.Expressions;

namespace MobID.MainGateway.Repo;

public class MainDbContext : DbContext
{
    public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<OrganizationUser> OrganizationUsers => Set<OrganizationUser>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Access> Accesses => Set<Access>();
    public DbSet<AccessType> AccessTypes => Set<AccessType>();
    public DbSet<QrCode> QrCodes => Set<QrCode>();
    public DbSet<Scan> Scans => Set<Scan>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserAccess> UserAccesses => Set<UserAccess>();
    public DbSet<OrganizationAccessShare> OrganizationAccessShares => Set<OrganizationAccessShare>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // UserRole
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

        // OrganizationUser
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

        // Access
        modelBuilder.Entity<Access>()
            .HasOne(a => a.Organization)
            .WithMany(o => o.Accesses)
            .HasForeignKey(a => a.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Access>()
            .HasOne(a => a.CreatedByUser)
            .WithMany()
            .HasForeignKey(a => a.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Access>()
           .HasOne(a => a.UpdatedByUser)
           .WithMany()
           .HasForeignKey(a => a.UpdatedByUserId)
           .OnDelete(DeleteBehavior.Restrict);

        // QrCode
        modelBuilder.Entity<QrCode>()
            .HasOne(q => q.Access)
            .WithMany(a => a.QrCodes)
            .HasForeignKey(q => q.AccessId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<QrCode>()
            .HasQueryFilter(q => q.DeletedAt == null);

        // Scan
        modelBuilder.Entity<Scan>()
            .HasOne(s => s.QrCode)
            .WithMany(q => q.Scans)
            .HasForeignKey(s => s.QrCodeId)
            .OnDelete(DeleteBehavior.Cascade);
        modelBuilder.Entity<Scan>()
            .HasQueryFilter(s => s.DeletedAt == null);

        // RefreshToken
        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithOne(u => u.RefreshToken)
            .HasForeignKey<RefreshToken>(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // UserAccess
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

        // OrganizationAccessShare (relații corecte)
        modelBuilder.Entity<OrganizationAccessShare>()
            .HasOne(s => s.SourceOrganization)
            .WithMany(o => o.AccessSharesAsSource)
            .HasForeignKey(s => s.SourceOrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrganizationAccessShare>()
            .HasOne(s => s.TargetOrganization)
            .WithMany(o => o.AccessSharesAsTarget)
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
            .HasForeignKey(s => s.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrganizationAccessShare>()
            .HasQueryFilter(s => s.DeletedAt == null);

        // AccessType seed
        modelBuilder.Entity<AccessType>().HasData(
            new AccessType
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Name = AccessTypeCode.OneUse.ToString(),
                Description = "Valabil o singură scanare",
                Code = AccessTypeCode.OneUse
            },
            new AccessType
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Name = AccessTypeCode.LimitedUse.ToString(),
                Description = "Ex: 8 scanări. Se scade la fiecare utilizare.",
                Code = AccessTypeCode.LimitedUse
            },
            new AccessType
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Name = AccessTypeCode.Subscription.ToString(),
                Description = "Se resetează lunar, opțional cu limită",
                Code = AccessTypeCode.Subscription
            },
            new AccessType
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000004"),
                Name = AccessTypeCode.Unlimited.ToString(),
                Description = "Acces complet fără restricții",
                Code = AccessTypeCode.Unlimited
            }
        );

        // Role seed
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

        // User + UserRole seed
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
}
