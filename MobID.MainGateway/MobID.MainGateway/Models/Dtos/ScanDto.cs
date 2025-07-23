namespace MobID.MainGateway.Models.Dtos;

public class ScanDto
{
    public Guid Id { get; }
    public Guid QrCodeId { get; }
    public Guid ScannedByUserId { get; }
    public string ScannedByUserName { get; }
    public Guid ScannedForUserId { get; }
    public string ScannedForUserName { get; }
    public bool IsSuccessfull { get; set; }

    public DateTime ScannedAt { get;}

    public ScanDto(Scan s)
    {
        Id = s.Id;
        QrCodeId = s.QrCodeId;
        ScannedByUserId = s.ScannedByUserId;
        ScannedByUserName = s.ScannedByUser?.Username ?? "–";
        ScannedForUserId = s.ScannedForUserId;
        ScannedForUserName = s.ScannedForUser?.Username ?? "–";
        ScannedAt = s.CreatedAt;
        IsSuccessfull = s.IsSuccessfull;
    }
}
