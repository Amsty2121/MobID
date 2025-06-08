using MobID.MainGateway.Models.Enums;

namespace MobID.MainGateway.Models.Entities;

public class AccessType : IBaseEntity
{
    public Guid Id { get; set; }

    /// <summary>
    /// Numele tipului de acces (ex: "OneUse", "LimitedUse").
    /// </summary>
    public string Name { get; set; } = null!;

    /// <summary>
    /// Descrierea tipului de acces – apare în UI.
    /// </summary>
    public string? Description { get; set; }

    public AccessTypeCode Code { get; set; }

    // Audit
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    // Navigație inversă
    public ICollection<Access> Accesses { get; set; } = new List<Access>();
}
