using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Models.Dtos;

public class UserAccessDto
{
    public Guid UserId { get; }
    public string UserName { get; }
    public Guid GrantedById { get; }
    public string GrantedByName { get; }
    public DateTime GrantedAt { get; }
    public AccessDto Access { get; }

    public UserAccessDto(UserAccess ua)
    {
        UserId = ua.UserId;
        UserName = ua.User?.Username ?? "–";
        GrantedAt = ua.CreatedAt;
    }
}
