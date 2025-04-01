using MobID.MainGateway.Models.Enums;

namespace MobID.MainGateway.Models.Entities
{
    public class Access : IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CreatedBy { get; set; }
        public User Creator { get; set; }
        public Guid AccessTypeId { get; set; }
        public AccessType AccessType { get; set; }
        public ScanMode ScanMode { get; set; }
        public int? MaxUses { get; set; }
        public int? MaxUsersPerPass { get; set; }
        public int? MonthlyLimit { get; set; }
        public int? TotalUses { get; set; }
        public int? MaxUsersPerUsage { get; set; }
        public DateTime? ExpirationDateTime { get; set; }

        public bool RestrictToOrganizationMembers { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
        public ICollection<QrCode> QrCodes { get; set; }
    }
}
