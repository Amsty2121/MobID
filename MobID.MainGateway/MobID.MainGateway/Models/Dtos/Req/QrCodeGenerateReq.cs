using System.ComponentModel.DataAnnotations;
using MobID.MainGateway.Models.Enums;

namespace MobID.MainGateway.Models.Dtos.Req;

public class QrCodeGenerateReq
{
    [Required]
    public Guid AccessId { get; set; }

    [StringLength(200)]
    public string? Description { get; set; }

    [Required]
    [EnumDataType(typeof(QrCodeType))]
    public string Type { get; set; }

    public DateTime? ExpiresAt { get; set; }
}
