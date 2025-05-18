using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Enums;

namespace MobID.MainGateway.Models.Dtos;

public class AccessDto
{
    public Guid Id { get; }
    public Guid OrganizationId { get; }
    public string OrganizationName { get; }

    public bool IsActive { get; }
    public Guid CreatedBy { get; }
    public string CreatorName { get; }

    public string AccessType { get; }
    public string ScanMode { get; }

    public string Description { get; }

    public int? MaxUses { get; }
    public int? MaxUsersPerPass { get; }
    public int? MonthlyLimit { get; }
    public int? SubscriptionPeriodMonths { get; }
    public int? UsesPerPeriod { get; }

    public DateTime? ExpirationDateTime { get; }
    public bool RestrictToOrganizationMembers { get; }

    public List<Guid> QrCodeIds { get; }
    public bool IsDeleted { get; }

    public AccessDto(Access access)
    {
        Id = access.Id;
        OrganizationId = access.OrganizationId;
        OrganizationName = access.Organization?.Name ?? "—";

        IsActive = access.IsActive && access.DeletedAt == null;
        CreatedBy = access.CreatedBy;
        CreatorName = access.Creator?.Username ?? "—";

        AccessType = access.AccessType?.Name ?? "—";
        ScanMode = access.ScanMode.ToString();
        Description = access.Description;

        MaxUses = access.MaxUses;
        MaxUsersPerPass = access.MaxUsersPerPass;
        MonthlyLimit = access.MonthlyLimit;
        SubscriptionPeriodMonths = access.SubscriptionPeriodMonths;
        UsesPerPeriod = access.UsesPerPeriod;

        ExpirationDateTime = access.ExpirationDateTime;
        RestrictToOrganizationMembers = access.RestrictToOrganizationMembers;

        QrCodeIds = access.QrCodes is null
                                          ? new List<Guid>()
                                          : new List<Guid>(access.QrCodes.Select(q => q.Id));

        IsDeleted = access.DeletedAt != null;
    }
}
