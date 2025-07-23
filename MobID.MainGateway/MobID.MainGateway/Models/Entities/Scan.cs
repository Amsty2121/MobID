using MobID.MainGateway.Models.Entities;

public class Scan : IBaseEntity
{
    public Guid Id { get; set; }

    public Guid ScannedByUserId { get; set; }
    public User ScannedByUser { get; set; }

    public Guid ScannedForUserId { get; set; }
    public User ScannedForUser { get; set; }

    public Guid QrCodeId { get; set; }
    public QrCode QrCode { get; set; }

    public bool IsSuccessfull { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
}
