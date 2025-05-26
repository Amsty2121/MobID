using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Models.Dtos
{
    public class QrCodeDto
    {
        public Guid Id { get; }
        public Guid AccessId { get; }
        public string Description { get; }
        public bool IsActive { get; }
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; }

        //–– toate scanările asociate
        public List<ScanDto> Scans { get; }

        public QrCodeDto(QrCode qr)
        {
            Id = qr.Id;
            AccessId = qr.AccessId;
            Description = qr.Description;
            IsActive = qr.IsActive && qr.DeletedAt == null;
            CreatedAt = qr.CreatedAt;
            UpdatedAt = qr.UpdatedAt;

            Scans = qr.Scans?.Select(s => new ScanDto(s)).ToList()
                 ?? new List<ScanDto>();
        }
    }
}
