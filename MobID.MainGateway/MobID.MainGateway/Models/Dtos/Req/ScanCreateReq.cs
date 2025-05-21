using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class ScanCreateReq
{
    [Required]
    public Guid ScannedById { get; set; }

    [Required]
    public Guid QrCodeId { get; set; }
}
