using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;

namespace MobID.MainGateway.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> AddUser(UserAddReq userDto, CancellationToken ct = default);
        Task<bool> DeleteUser(Guid userId, CancellationToken ct = default);
        Task<UserDto?> GetUserById(Guid userId, CancellationToken ct = default);
        Task<PagedResponse<UserDto>> GetUsersPaged(PagedRequest pagedRequest, CancellationToken ct = default);
        Task<bool> AssignRoleToUser(Guid userId, Guid roleId, CancellationToken ct = default);
        Task<bool> RemoveRoleFromUser(Guid userId, Guid roleId, CancellationToken ct = default);
        Task<List<string>> GetUserRoles(Guid userId, CancellationToken ct = default);
    }
}
