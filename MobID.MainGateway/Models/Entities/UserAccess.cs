using MobID.MainGateway.Models.Enums;

namespace MobID.MainGateway.Models.Entities;

public class UserAccess : IBaseEntity
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; }

    // La ce acces
    public Guid AccessId { get; set; }
    public Access Access { get; set; }

    public Guid GrantedByUserId { get; set; }
    public User GrantedByUser { get; set; }

    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;

    public AccessGrantType GrantType { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
}
