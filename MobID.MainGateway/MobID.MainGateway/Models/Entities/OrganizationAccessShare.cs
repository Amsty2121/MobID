namespace MobID.MainGateway.Models.Entities;

public class OrganizationAccessShare : IBaseEntity
{
    public Guid Id { get; set; }

    public Guid SourceOrganizationId { get; set; }
    public Organization SourceOrganization { get; set; }

    public Guid TargetOrganizationId { get; set; }
    public Organization TargetOrganization { get; set; }

    public Guid AccessId { get; set; }
    public Access Access { get; set; }

    public Guid CreatedBy { get; set; }
    public User Creator { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
}
