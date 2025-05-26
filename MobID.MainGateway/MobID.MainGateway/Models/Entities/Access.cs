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

    /// <summary>
    /// Numărul maxim total de scanări (pentru LimitedUse).
    /// Pentru IdentityConfirm, egal 1.
    /// Pentru Subscription nelimitat => null.
    /// </summary>
    public int? TotalUseLimit { get; set; }

    /// <summary>
    /// Numărul maxim de scanări per perioadă (pentru Subscription).
    /// Dacă e null, înseamnă „nelimitat” în perioada definită.
    /// </summary>
    public int? UseLimitPerPeriod { get; set; }

    /// <summary>
    /// Durata perioadei de abonament (pentru Subscription).
    /// Ex: TimeSpan.FromDays(7), FromMonths(1), etc.
    /// </summary>
    public TimeSpan? SubscriptionPeriod { get; set; }

    /// <summary>
    /// Data până la care codul e valid; dacă e null => fără expirare.
    /// </summary>
    public DateTime? ExpirationDateTime { get; set; }

    public bool IsRestrictedToOrgMembers { get; set; } = false;
    public bool IsRestrictedToOrganizationShare { get; set; } = false;

    public Guid CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    // Relații
    public ICollection<QrCode> QrCodes { get; set; }
    public ICollection<Scan> Scans { get; set; }
    public ICollection<UserAccess> UserAccesses { get; set; }
    public ICollection<OrganizationAccessShare> OrganizationAccessShares { get; set; }
}
