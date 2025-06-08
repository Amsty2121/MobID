namespace MobID.MainGateway.Models.Entities;

public class OrganizationAccessShare : IBaseEntity
{
    public Guid Id { get; set; }

    public Guid SourceOrganizationId { get; set; }
    public Organization SourceOrganization { get; set; } = null!;

    public Guid TargetOrganizationId { get; set; }
    public Organization TargetOrganization { get; set; } = null!;

    public Guid AccessId { get; set; }
    public Access Access { get; set; } = null!;

    public Guid CreatedByUserId { get; set; }
    public User Creator { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
}
