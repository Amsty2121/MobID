using MobID.MainGateway.Models.Entities;

public class Scan : IBaseEntity
{
    public Guid Id { get; set; }

    public Guid ScannedById { get; set; }
    public User ScannedBy { get; set; }

    public Guid QrCodeId { get; set; }
    public QrCode QrCode { get; set; }

    public DateTime ScannedAt { get; set; } = DateTime.UtcNow;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
}
