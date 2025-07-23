using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class ScanQrReq
{
    [Required]
    public string QrRawValue {  get; set; }
}
