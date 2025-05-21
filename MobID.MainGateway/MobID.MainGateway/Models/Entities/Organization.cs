namespace MobID.MainGateway.Models.Entities
{
    public class Organization : IBaseEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public Guid OwnerId {  get; set; }
        public User Owner { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }

        public ICollection<OrganizationUser> OrganizationUsers { get; set; }
        public ICollection<OrganizationAccessShare> OrganizationAccessShares { get; set; }

    }
}
