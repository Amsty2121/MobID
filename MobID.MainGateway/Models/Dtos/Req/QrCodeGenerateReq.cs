using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class QrCodeGenerateReq
{
    [Required]
    public Guid AccessId { get; set; }
}
