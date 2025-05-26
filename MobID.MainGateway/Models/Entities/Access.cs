using MobID.MainGateway.Models.Enums;

namespace MobID.MainGateway.Models.Entities;

public class Access : IBaseEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string Description { get; set; }

    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; }

    public Guid AccessTypeId { get; set; }
    public AccessType AccessType { get; set; }

    public bool IsActive { get; set; } = true;
    public Guid CreatedBy { get; set; }
    public User Creator { get; set; }

    // Redenumit
    public AccessNumberScanMode ScanMode { get; set; }

    // Tipuri de limitări
    public int? MaxUses { get; set; }
    public int? MaxUsersPerPass { get; set; }

    // Pentru subscription
    public int? MonthlyLimit { get; set; }
    public int? SubscriptionPeriodMonths { get; set; }
    public int? UsesPerPeriod { get; set; }

    public DateTime? ExpirationDateTime { get; set; }
    public bool RestrictToOrganizationMembers { get; set; } = false;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    // Relații
    public ICollection<QrCode> QrCodes { get; set; }
    public ICollection<Scan> Scans { get; set; }
    public ICollection<UserAccess> UserAccesses { get; set; }
    public ICollection<OrganizationAccessShare> OrganizationAccessShares { get; set; }

}