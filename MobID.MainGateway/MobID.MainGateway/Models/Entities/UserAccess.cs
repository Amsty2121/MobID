namespace MobID.MainGateway.Models.Entities;

public class UserAccess : IBaseEntity
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid AccessId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public User User { get; set; }
    public Access Access { get; set; }
}
