using MobID.MainGateway.Models.Dtos.Req.Validators;
using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req
{
    public class UserRegisterReq
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [ValidRoles(ErrorMessage = "One or more roles are invalid.")]
        public ICollection<string> Roles { get; set; }

    }
}
