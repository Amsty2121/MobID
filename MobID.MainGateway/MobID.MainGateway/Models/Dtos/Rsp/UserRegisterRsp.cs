using System.Collections.Generic;
using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Models.Dtos.Rsp
{
    public class UserRegisterRsp
    {
        public Guid Id { get; }
        public string Email { get; }
        public string Username { get; }
        public ICollection<string> Roles { get; }

        public UserRegisterRsp(User user, ICollection<string> roles)
        {
            Id = user.Id;
            Email = user.Email;
            Username = user.Username;
            Roles = roles;
        }
    }
}
