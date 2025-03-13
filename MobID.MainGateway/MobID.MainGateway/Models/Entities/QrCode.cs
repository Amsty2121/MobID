namespace MobID.MainGateway.Models.Entities
{
    public class QrCode : IBaseEntity
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }

        public Guid AccessId { get; set; }
        public Access Access { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
    }
}
