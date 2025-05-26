using MobID.MainGateway.Models.Entities;
using System.Text.Json;
using System.Text;

namespace MobID.MainGateway.Helpers;

public static class QrCodeContentGenerator
{
    public static string GenerateBase64Payload(QrCode qr)
    {
        var payload = new
        {
            qrId = qr.Id,
            expiresAt = qr.ExpiresAt?.ToUniversalTime().ToString("o") // ISO 8601
        };

        var json = JsonSerializer.Serialize(payload);
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
    }
}
