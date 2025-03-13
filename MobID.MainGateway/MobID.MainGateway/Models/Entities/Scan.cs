using MobID.MainGateway.Models.Entities;

public class Scan : IBaseEntity
{
    public Guid Id { get; set; }

    public Guid AccessId { get; set; }
    public Access Access { get; set; }

    public Guid ScannedById { get; set; }
    public User ScannedBy { get; set; } // Cine a scanat codul QR

    public DateTime ScannedAt { get; set; } = DateTime.UtcNow; // Când a fost scanat

    public Guid? QrCodeId { get; set; } // Ce cod QR a fost scanat
    public QrCode QrCode { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? DeletedAt { get; set; }
}
