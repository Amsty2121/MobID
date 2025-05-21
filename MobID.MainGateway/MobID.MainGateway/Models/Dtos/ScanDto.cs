namespace MobID.MainGateway.Models.Dtos;

public class ScanDto
{
    public Guid Id { get; }
    public Guid QrCodeId { get; }
    public Guid ScannedById { get; }
    public string ScannedByName { get; }
    public DateTime ScannedAt { get; }

    public ScanDto(Scan s)
    {
        Id = s.Id;
        QrCodeId = s.QrCodeId;
        ScannedById = s.ScannedById;
        ScannedByName = s.ScannedBy?.Username ?? "–";
        ScannedAt = s.ScannedAt;
    }
}
