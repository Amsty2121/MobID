using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Rsp;

namespace MobID.MainGateway.Services.Interfaces;

public interface IRoleService
{
    Task<RoleDto> CreateRoleAsync(string name, string description, CancellationToken ct = default);
    Task<RoleDto?> GetRoleByIdAsync(Guid roleId, CancellationToken ct = default);
    Task<RoleDto?> GetRoleByNameAsync(string roleName, CancellationToken ct = default);
    Task<List<RoleDto>> GetAllRolesAsync(CancellationToken ct = default);
    Task<PagedResponse<RoleDto>> GetRolesPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<bool> DeactivateRoleAsync(Guid roleId, CancellationToken ct = default);
}
