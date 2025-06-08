using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class AccessUpdateReq
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public DateTime? ExpirationDateTime { get; set; }

    public bool RestrictToOrgMembers { get; set; }

    public bool RestrictToOrgSharing { get; set; }

    public bool IsMultiScan { get; set; }

    public int? TotalUseLimit { get; set; }
    public int? SubscriptionPeriodMonths { get; set; }
    public int? UseLimitPerPeriod { get; set; }
}
