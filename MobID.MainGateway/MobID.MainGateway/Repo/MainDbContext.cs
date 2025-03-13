using Microsoft.EntityFrameworkCore;
using MobID.MainGateway.Models.Entities;
using System.Linq.Expressions;

namespace MobID.MainGateway.Repo
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options) : base(options) { }

        // 🔹 Definim entitățile ca DbSet<>
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
            // 🔹 Configurăm relațiile

            // Organization -> Users (many-to-many prin OrganizationUser)
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

            // Users -> Roles (many-to-many prin UserRole)
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

            // Access -> Organization (many-to-one)
            modelBuilder.Entity<Access>()
                .HasOne(a => a.Organization)
                .WithMany()
                .HasForeignKey(a => a.OrganizationId);

            // Access -> Creator (User)
            modelBuilder.Entity<Access>()
                .HasOne(a => a.Creator)
                .WithMany()
                .HasForeignKey(a => a.CreatedBy);

            // QrCode -> Access (many-to-one)
            modelBuilder.Entity<QrCode>()
                .HasOne(qr => qr.Access)
                .WithMany(a => a.QrCodes)
                .HasForeignKey(qr => qr.AccessId);

            // Scan -> Access (many-to-one)
            modelBuilder.Entity<Scan>()
                .HasOne(s => s.Access)
                .WithMany()
                .HasForeignKey(s => s.AccessId);

            // Scan -> QrCode (many-to-one, optional)
            modelBuilder.Entity<Scan>()
                .HasOne(s => s.QrCode)
                .WithMany()
                .HasForeignKey(s => s.QrCodeId)
                .IsRequired(false);

            // Scan -> User (many-to-one)
            modelBuilder.Entity<Scan>()
                .HasOne(s => s.ScannedBy)
                .WithMany()
                .HasForeignKey(s => s.ScannedById);

            modelBuilder.Entity<RefreshToken>()
                .HasQueryFilter(rt => rt.User.DeletedAt == null);


            // 🔹 Soft Delete: Nu ștergem rândurile cu DeletedAt != null
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (entityType.ClrType.GetProperty("DeletedAt") != null)
                {
                    modelBuilder.Entity(entityType.ClrType)
                        .HasQueryFilter(ConvertFilterExpression(entityType.ClrType));
                }
            }
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
