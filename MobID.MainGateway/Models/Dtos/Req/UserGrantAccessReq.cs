using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class UserGrantAccessReq
{
    [Required]
    public Guid AccessId { get; set; }

    [Required]
    public Guid TargetUserId { get; set; }

    [Required]
    public Guid FromOrganizationId { get; set; }
}
