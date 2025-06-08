using MobID.MainGateway.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobID.MainGateway.Models.Entities;

public class OrganizationUser : IBaseEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;

    public Guid OrganizationId { get; set; }
    public Organization Organization { get; set; } = null!;

    public OrganizationUserRole Role { get; set; } = OrganizationUserRole.Member;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    [NotMapped]
    public bool IsActive => DeletedAt == null;
}
