using System.ComponentModel.DataAnnotations;
using MobID.MainGateway.Models.Enums;

namespace MobID.MainGateway.Models.Dtos.Req;

public class QrCodeGenerateReq
{
    [Required]
    public Guid AccessId { get; set; }

    public string Description { get; set; }

    public QrCodeType Type { get; set; } = QrCodeType.Access;

    public DateTime? ExpiresAt { get; set; }
}
