using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class OrganizationUpdateReq
{
    [Required]
    public Guid OrganizationId { get; set; }

    [MinLength(2), MaxLength(100)]
    public string? Name { get; set; }

    public Guid? OwnerId { get; set; }
}
