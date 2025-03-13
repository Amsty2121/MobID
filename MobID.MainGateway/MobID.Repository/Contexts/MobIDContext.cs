using Microsoft.EntityFrameworkCore;
using SmartPass.Repository.Models.Entities;

namespace SmartPass.Repository.Contexts
{
    public class SmartPassContext : DbContext
    {
        public SmartPassContext(DbContextOptions<SmartPassContext> options) : base(options)
        { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserAuthData> UserAuthDatas { get; set; }
        public DbSet<AccessLevel> AccessLevels { get; set; }
        public DbSet<AccessCard> AccessCards { get; set; }
        public DbSet<CardReader> CardReaders { get; set; }
        public DbSet<AccessLog> AccessLog { get; set; }
        public DbSet<Zone> Zones { get; set; }

        /*protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserAuthData)
                .WithOne(ua => ua.User)
                .HasForeignKey<UserAuthData>(ua => ua.Id);

            base.OnModelCreating(modelBuilder);
        }*/
    }

}
