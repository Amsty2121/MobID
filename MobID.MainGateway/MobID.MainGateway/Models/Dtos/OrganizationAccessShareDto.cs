using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Models.Dtos;

public class OrganizationAccessShareDto
{
    public Guid Id { get; }
    public Guid SourceOrganizationId { get; }
    public string SourceOrganizationName { get; }
    public Guid TargetOrganizationId { get; }
    public string TargetOrganizationName { get; }
    public Guid AccessId { get; }
    public string AccessName { get; }
    public Guid CreatedBy { get; }
    public string CreatedByName { get; }
    public DateTime CreatedAt { get; }

    public OrganizationAccessShareDto(OrganizationAccessShare s)
    {
        Id = s.Id;
        SourceOrganizationId = s.SourceOrganizationId;
        SourceOrganizationName = s.SourceOrganization?.Name ?? "–";
        TargetOrganizationId = s.TargetOrganizationId;
        TargetOrganizationName = s.TargetOrganization?.Name ?? "–";
        AccessId = s.AccessId;
        AccessName = s.Access?.Name ?? "–";
        CreatedBy = s.CreatedBy;
        CreatedByName = s.Creator?.Username ?? "–";
        CreatedAt = s.CreatedAt;
    }
}
