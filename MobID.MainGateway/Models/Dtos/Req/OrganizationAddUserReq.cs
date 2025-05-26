using MobID.MainGateway.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class OrganizationAddUserReq
{
    [Required]
    public Guid UserId { get; set; }

    [EnumDataType(typeof(OrganizationUserRole))]
    public string? Role { get; set; }
}
