using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Models.Dtos;

public class OrganizationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }

    public Guid OwnerId { get; set; }
    public string OwnerUsername { get; set; }

    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public OrganizationDto(Organization org)
    {
        Id = org.Id;
        Name = org.Name;
        Description = org.Description;
        OwnerId = org.OwnerId;
        OwnerUsername = org.Owner?.Username ?? "N/A";
        CreatedAt = org.CreatedAt;
        IsActive = org.DeletedAt == null;
    }
}
