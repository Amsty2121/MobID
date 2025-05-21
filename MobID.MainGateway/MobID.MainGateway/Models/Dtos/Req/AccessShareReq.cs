using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class AccessShareReq
{
    [Required]
    public Guid AccessId { get; set; }

    [Required]
    public Guid FromOrganizationId { get; set; }

    [Required]
    public Guid ToOrganizationId { get; set; }
}
