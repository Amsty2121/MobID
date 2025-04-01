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

        public QrCodeDto(QrCode qrCode)
        {
            Id = qrCode.Id;
            AccessId = qrCode.AccessId;
            Description = qrCode.Description;

            IsActive = qrCode.IsActive && qrCode.DeletedAt == null;

            CreatedAt = qrCode.CreatedAt;
            UpdatedAt = qrCode.UpdatedAt;
        }
    }
}
