using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class AccessCreateReq
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public Guid OrganizationId { get; set; }

    [Required]
    public Guid AccessTypeId { get; set; }

    public int? TotalUseLimit { get; set; }

    public int? SubscriptionPeriodMonths { get; set; }

    public int? UseLimitPerPeriod { get; set; }

    public DateTime? ExpirationDateTime { get; set; }

    public bool RestrictToOrgMembers { get; set; }

    public bool RestrictToOrgSharing { get; set; }

    public bool IsMultiScan { get; set; }
}
