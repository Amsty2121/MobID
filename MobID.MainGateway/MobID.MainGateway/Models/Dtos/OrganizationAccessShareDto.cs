using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Models.Dtos;

public class OrganizationAccessShareDto
{
    public Guid Id { get; set; }

    public Guid SourceOrganizationId { get; set; }
    public string SourceOrganizationName { get; set; }

    public Guid TargetOrganizationId { get; set; }
    public string TargetOrganizationName { get; set; }

    public Guid AccessId { get; set; }
    public string AccessName { get; set; }

    public Guid CreatedByUserId { get; set; }
    public string CreatedByUsername { get; set; }

    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public OrganizationAccessShareDto(OrganizationAccessShare share)
    {
        Id = share.Id;
        SourceOrganizationId = share.SourceOrganizationId;
        SourceOrganizationName = share.SourceOrganization?.Name ?? "N/A";
        TargetOrganizationId = share.TargetOrganizationId;
        TargetOrganizationName = share.TargetOrganization?.Name ?? "N/A";
        AccessId = share.AccessId;
        AccessName = share.Access?.Name ?? "N/A";
        CreatedByUserId = share.CreatedByUserId;
        CreatedByUsername = share.Creator?.Username ?? "N/A";
        CreatedAt = share.CreatedAt;
        IsActive = share.DeletedAt == null;
    }
}
