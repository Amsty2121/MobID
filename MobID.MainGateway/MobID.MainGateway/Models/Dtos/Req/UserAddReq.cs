using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req;

public class UserAddReq
{
    [Required, EmailAddress]
    public string Email { get; set; }

    [Required, MinLength(3), MaxLength(50)]
    public string Username { get; set; }

    [Required, MinLength(6), MaxLength(100)]
    public string Password { get; set; }
}
