using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class AccessCreateReq
{
    [Required]
    public Guid AccessTypeId { get; set; }

    [Required]
    public Guid OrganizationId { get; set; }

    [Required, StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }

    [StringLength(500)]
    public string Description { get; set; }

    /// <summary>
    /// Pentru LimitedUse: numărul total de scanări permise (>=1).
    /// Pentru IdentityConfirm, va fi forțat la 1.
    /// Pentru Subscription, poate fi null = nelimitat.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "TotalUseLimit must be at least 1 when specified.")]
    public int? TotalUseLimit { get; set; } = 1;

    /// <summary>
    /// Pentru Subscription: numărul de scanări per perioadă (>=1) sau null = nelimitat.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "UseLimitPerPeriod must be at least 1 when specified.")]
    public int? UseLimitPerPeriod { get; set; } = 1;

    /// <summary>
    /// Pentru Subscription: durata perioadei (ex: TimeSpan.FromDays(7), FromMonths(1), etc.).
    /// </summary>
    public TimeSpan? SubscriptionPeriod { get; set; }

    /// <summary>
    /// Orice tip de acces poate avea o dată de expirare după care nu mai este valid.
    /// </summary>
    public DateTime? ExpirationDate { get; set; }

    public bool RestrictToOrgMembers { get; set; }
    public bool RestrictToOrganizationShare { get; set; }
}
