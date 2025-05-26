using MobID.MainGateway.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class AccessCreateReq
{
    [Required, MinLength(2), MaxLength(100)]
    public string Name { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [Required]
    public Guid OrganizationId { get; set; }

    [Required]
    public Guid AccessTypeId { get; set; }

    [DataType(DataType.Date)]
    public DateTime? ExpirationDate { get; set; }

    public bool RestrictToOrganizationMembers { get; set; }

    [Required]
    [EnumDataType(typeof(AccessNumberScanMode))]
    public string ScanMode { get; set; }

    [Range(1, int.MaxValue)]
    public int? MaxUses { get; set; }

    [Range(1, int.MaxValue)]
    public int? MaxUsersPerPass { get; set; }

    [Range(1, int.MaxValue)]
    public int? MonthlyLimit { get; set; }

    [Range(1, 60)]
    public int? SubscriptionPeriodMonths { get; set; }

    [Range(1, int.MaxValue)]
    public int? UsesPerPeriod { get; set; }
    
    [Required]
    public Guid CreatedBy { get; set; }
}