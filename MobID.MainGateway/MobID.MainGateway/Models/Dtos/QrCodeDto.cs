using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Enums;
using MobID.MainGateway.Helpers;

namespace MobID.MainGateway.Models.Dtos
{
    public class QrCodeDto
    {
        public Guid Id { get; }
        public string Description { get; }
        public string Type { get; }
        public Guid AccessId { get; }
        public DateTime? ExpiresAt { get; }
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; }

        public string QREncodedText { get; }

        public List<ScanDto> Scans { get; }

        public QrCodeDto(QrCode qr)
        {
            Id = qr.Id;
            Description = qr.Description;
            Type = qr.Type.ToString();
            AccessId = qr.AccessId;
            ExpiresAt = qr.ExpiresAt;
            CreatedAt = qr.CreatedAt;
            UpdatedAt = qr.UpdatedAt;

            QREncodedText = QrCodeContentGenerator.GenerateBase64Payload(qr);

            Scans = qr.Scans?.Select(s => new ScanDto(s)).ToList()
                 ?? new List<ScanDto>();
        }
    }
}
