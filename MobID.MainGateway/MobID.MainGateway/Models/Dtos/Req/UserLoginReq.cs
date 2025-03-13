using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req
{
    public class UserLoginReq
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
