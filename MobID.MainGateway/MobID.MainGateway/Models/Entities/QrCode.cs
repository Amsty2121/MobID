using MobID.MainGateway.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobID.MainGateway.Models.Entities;

public class QrCode : IBaseEntity
{
    public Guid Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public QrCodeType Type { get; set;  }

    public Guid AccessId { get; set; }
    public Access Access { get; set; } = null!;

    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }

    public ICollection<Scan> Scans { get; set; } = new List<Scan>();

    [NotMapped]
    public bool IsExpired => ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;

    [NotMapped]
    public bool IsActive => DeletedAt == null && !IsExpired;
}
