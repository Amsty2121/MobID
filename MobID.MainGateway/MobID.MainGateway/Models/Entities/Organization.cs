using System.ComponentModel.DataAnnotations.Schema;

namespace MobID.MainGateway.Models.Entities;

public class Organization : IBaseEntity
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public string? Description { get; set; }

    public Guid OwnerId { get; set; }
    public User Owner { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    // Navigații directe
    public ICollection<OrganizationUser> OrganizationUsers { get; set; } = new List<OrganizationUser>();
    public ICollection<Access> Accesses { get; set; } = new List<Access>();

    // Navigații partajări
    public ICollection<OrganizationAccessShare> AccessSharesAsSource { get; set; } = new List<OrganizationAccessShare>();
    public ICollection<OrganizationAccessShare> AccessSharesAsTarget { get; set; } = new List<OrganizationAccessShare>();
}
