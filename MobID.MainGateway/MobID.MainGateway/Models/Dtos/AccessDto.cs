using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Models.Dtos;
public class AccessDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }

    public Guid OrganizationId { get; set; }
    public string OrganizationName { get; set; }
    public Guid AccessTypeId { get; set; }
    public string AccessTypeName { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; }
    public Guid UpdatedByUserId { get; set; }
    public string UpdatedByUserName { get; set; }


    public int? TotalUseLimit { get; set; }
    public int? SubscriptionPeriodMonths { get; set; }
    public int? UseLimitPerPeriod { get; set; }

    public DateTime? ExpirationDateTime { get; set; }
    public bool RestrictToOrgMembers { get; set; }
    public bool RestrictToOrgSharing { get; set; }

    public bool IsMultiScan { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public AccessDto(Access access)
    {
        Id = access.Id;
        Name = access.Name;
        Description = access.Description;

        OrganizationId = access.OrganizationId;
        OrganizationName = access.Organization?.Name ?? "N/A";
        AccessTypeId = access.AccessTypeId;
        AccessTypeName = access.AccessType?.Name ?? "N/A";
        CreatedByUserId = access.CreatedByUserId;
        CreatedByUserName = access.CreatedByUser?.Username ?? "N/A";
        UpdatedByUserId = access.UpdatedByUserId;
        UpdatedByUserName = access.UpdatedByUser?.Username ?? "N/A";


        TotalUseLimit = access.TotalUseLimit;
        SubscriptionPeriodMonths = access.SubscriptionPeriodMonths;
        UseLimitPerPeriod = access.UseLimitPerPeriod;

        ExpirationDateTime = access.ExpirationDateTime;

        RestrictToOrgMembers = access.RestrictToOrgMembers;
        RestrictToOrgSharing = access.RestrictToOrgSharing;
        IsMultiScan = access.IsMultiScan;

        IsActive = access.IsActive;
        CreatedAt = access.CreatedAt;
    }
}
