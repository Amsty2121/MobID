using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Models.Dtos.Rsp
{
    public class UserLoginRsp
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }

        public UserLoginRsp(User user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            Username = user.Username;
            Token = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
