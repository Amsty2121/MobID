using MobID.MainGateway.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobID.MainGateway.Models.Entities;

public class Access : IBaseEntity
{
    public Guid Id { get; set; }

    // === Informații generale ===

    public string Name { get; set; }
    public string? Description { get; set; }


    public Guid OrganizationId { get; set; }
    public Guid AccessTypeId { get; set; }
    public Guid CreatedByUserId { get; set; }
    public Guid UpdatedByUserId { get; set; }

    // === Reguli de utilizare ===

    public int? TotalUseLimit { get; set; }
    public int? SubscriptionPeriodMonths { get; set; }
    public int? UseLimitPerPeriod { get; set; }

    public DateTime? ExpirationDateTime { get; set; }
    public bool RestrictToOrgMembers { get; set; }
    public bool RestrictToOrgSharing { get; set; }

    /// <summary>
    /// Dacă este activă scanarea multiplă (true) sau o singură scanare (false).
    /// </summary>
    public bool IsMultiScan { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    // === Calculat (nepersistat) ===
    [NotMapped]
    public bool IsActive => DeletedAt == null && (ExpirationDateTime == null || ExpirationDateTime > DateTime.UtcNow);

    
    // === Relații directe ===
    public Organization Organization { get; set; }
    public AccessType AccessType { get; set; }
    public User CreatedByUser { get; set; }
    public User UpdatedByUser { get; set; }

    // === Navigații ===

    public ICollection<QrCode> QrCodes { get; set; } = new List<QrCode>();
    public ICollection<Scan> Scans { get; set; } = new List<Scan>();
    public ICollection<UserAccess> UserAccesses { get; set; } = new List<UserAccess>();
    public ICollection<OrganizationAccessShare> OrganizationAccessShares { get; set; } = new List<OrganizationAccessShare>();
}
