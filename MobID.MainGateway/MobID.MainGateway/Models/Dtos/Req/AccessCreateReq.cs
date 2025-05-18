using MobID.MainGateway.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class AccessCreateReq
{
    [Required] public Guid OrganizationId { get; set; }
    [Required] public Guid AccessTypeId { get; set; }

    [StringLength(200)]
    public string Description { get; set; }

    public DateTime? ExpirationDate { get; set; }
    public bool RestrictToOrganizationMembers { get; set; }

    [Required]
    public string ScanMode { get; set; }

    // câmpuri opționale, validate în serviciu
    public int? MaxUses { get; set; }
    public int? MaxUsersPerPass { get; set; }
    public int? MonthlyLimit { get; set; }
    public int? SubscriptionPeriodMonths { get; set; }
    public int? UsesPerPeriod { get; set; }
}
