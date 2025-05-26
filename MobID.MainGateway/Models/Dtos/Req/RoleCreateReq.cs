using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class RoleCreateReq
{
    [Required]
    public string Name { get; set; }

    public string Description { get; set; }
}
