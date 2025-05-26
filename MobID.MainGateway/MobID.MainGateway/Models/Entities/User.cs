namespace MobID.MainGateway.Models.Entities
{
    public class User : IBaseEntity
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }


        public ICollection<UserRole> UserRoles { get; set; }
        public RefreshToken RefreshToken { get; set; }
        public ICollection<OrganizationUser> OrganizationUsers { get; set; }
        public ICollection<UserAccess> UserAccesses { get; set; }
    }
}
