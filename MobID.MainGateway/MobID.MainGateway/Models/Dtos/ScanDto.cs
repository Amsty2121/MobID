namespace MobID.MainGateway.Models.Dtos
{
    public class ScanDto
    {
        public Guid Id { get; }
        public Guid AccessId { get; }
        public Guid ScannedById { get; }
        public DateTime ScannedAt { get; }
        public Guid? QrCodeId { get; }

        public ScanDto(Scan scan)
        {
            Id = scan.Id;
            AccessId = scan.AccessId;
            ScannedById = scan.ScannedById;
            ScannedAt = scan.ScannedAt;
            QrCodeId = scan.QrCodeId;
        }
    }
}
