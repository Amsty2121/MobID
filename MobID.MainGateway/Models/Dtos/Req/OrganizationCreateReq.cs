using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class OrganizationCreateReq
{
    [Required, MinLength(2), MaxLength(100)]
    public string Name { get; set; }

    [Required]
    public Guid OwnerId { get; set; }
}
