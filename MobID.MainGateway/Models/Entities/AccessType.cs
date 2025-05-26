namespace MobID.MainGateway.Models.Entities
{
    public class AccessType : IBaseEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; } // Ex: "OneUse", "Subscription", "LimitedSubscription"
        public string Description { get; set; } // O scurtă descriere

        public bool IsLimitedUse { get; set; } // Dacă are limită de utilizări (ex: OneUse, MultiUse, LimitedSubscription)
        public bool IsSubscription { get; set; } // Dacă este un abonament (ex: Subscription, LimitedSubscription)

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }

        public ICollection<Access> Accesses { get; set; } // Relație one-to-many
    }
}
