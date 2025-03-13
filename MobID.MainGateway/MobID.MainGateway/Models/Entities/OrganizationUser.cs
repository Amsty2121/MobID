namespace MobID.MainGateway.Models.Entities
{
    public class OrganizationUser : IBaseEntity
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        public User User { get; set; }


        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
    }
}
