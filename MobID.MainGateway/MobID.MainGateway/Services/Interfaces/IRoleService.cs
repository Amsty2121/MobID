using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Rsp;

namespace MobID.MainGateway.Services.Interfaces
{
    public interface IRoleService
    {
        Task<RoleDto> AddRole(string roleName, string description, CancellationToken ct = default);
        Task<bool> DeleteRole(Guid roleId, CancellationToken ct = default);
        Task<RoleDto?> GetRoleById(Guid roleId, CancellationToken ct = default);
        Task<RoleDto?> GetRoleByName(string roleName, CancellationToken ct = default);
        Task<List<RoleDto>> GetAllRoles(CancellationToken ct = default);
        Task<PagedResponse<RoleDto>> GetRolesPaged(PagedRequest pagedRequest, CancellationToken ct = default);
    }
}
