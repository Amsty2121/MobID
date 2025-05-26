namespace MobID.MainGateway.Models.Entities
{
    public class AccessType : IBaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } 
        public string Description { get; set; } 

        public bool IsLimitedUse { get; set; } 
        public bool IsSubscription { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }

        public ICollection<Access> Accesses { get; set; } 
    }
}
