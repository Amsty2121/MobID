using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class UserLoginReq
{
    [Required, MinLength(3), MaxLength(100)]
    public string Login { get; set; }

    [Required, MinLength(6), MaxLength(100)]
    public string Password { get; set; }
}
