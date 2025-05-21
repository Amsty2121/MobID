using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;

namespace MobID.MainGateway.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateUserAsync(UserAddReq request, CancellationToken ct = default);
    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
    Task<PagedResponse<UserDto>> GetUsersPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<bool> DeactivateUserAsync(Guid userId, CancellationToken ct = default);

    Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken ct = default);
    Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken ct = default);
    Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken ct = default);
}
