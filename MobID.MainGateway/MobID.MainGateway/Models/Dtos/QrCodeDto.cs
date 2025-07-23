using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Enums;
using System.Text;

namespace MobID.MainGateway.Models.Dtos;

public class QrCodeDto
{
    public Guid Id { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }

    public Guid AccessId { get; set; }
    public string AccessName { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedAt { get; set; }

    public string QrEncodedText { get; set; }

    public QrCodeDto(QrCode qr)
    {
        Id = qr.Id;
        Description = qr.Description;
        Type = qr.Type.ToString();
        AccessId = qr.AccessId;
        AccessName = qr.Access?.Name ?? "N/A";
        CreatedAt = qr.CreatedAt;
        IsActive = qr.DeletedAt == null;

        var payload = $"{qr.Access!.OrganizationId}:{qr.AccessId}:{qr.Id}:{qr.Type}";
        QrEncodedText = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
    }
}
