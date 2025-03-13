using System.ComponentModel.DataAnnotations;

namespace MobID.MainGateway.Models.Dtos.Req
{
    public class UserAddReq
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
