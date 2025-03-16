using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Models.Dtos
{
    public class UserDto
    {
        public Guid Id { get; }
        public string Email { get; }
        public string Username { get; }
        public ICollection<string> Roles { get; }
        public bool IsDeleted { get; }


        public UserDto(User user, ICollection<string>? roles = null)
        {
            Id = user.Id;
            Email = user.Email;
            Username = user.Username;
            Roles = roles ?? new List<string>();
            IsDeleted = user.DeletedAt == null;
        }
    }
}
