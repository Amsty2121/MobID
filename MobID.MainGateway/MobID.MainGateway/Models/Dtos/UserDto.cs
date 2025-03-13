using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Models.Dtos
{
    public class UserDto
    {
        public Guid Id { get; }
        public string Email { get; }
        public string Username { get; }

        public UserDto(User user)
        {
            Id = user.Id;
            Email = user.Email;
            Username = user.Username;
        }
    }
}
