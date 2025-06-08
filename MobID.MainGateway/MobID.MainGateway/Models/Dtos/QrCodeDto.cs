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

    public bool IsExpired { get; set; }
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
        ExpiresAt = qr.ExpiresAt;
        CreatedAt = qr.CreatedAt;
        IsExpired = qr.ExpiresAt.HasValue && qr.ExpiresAt.Value <= DateTime.UtcNow;
        IsActive = qr.DeletedAt == null && !IsExpired;

        var payload = $"{qr.Access!.OrganizationId}:{qr.AccessId}:{qr.Id}";
        QrEncodedText = Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
    }
}
