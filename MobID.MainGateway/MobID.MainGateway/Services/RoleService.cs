using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Dtos.Rsp;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;
using MobID.MainGateway.Models.Dtos;

namespace MobID.MainGateway.Services
{
    public class RoleService : IRoleService
    {
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<UserRole> _userRoleRepository;

        public RoleService(IGenericRepository<Role> roleRepository, IGenericRepository<UserRole> userRoleRepository)
        {
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
        }

        public async Task<RoleDto> AddRole(string roleName, string description, CancellationToken ct = default)
        {
            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = roleName,
                Description = description,
                CreatedAt = DateTime.UtcNow
            };

            await _roleRepository.Add(role, ct);
            return new RoleDto(role);
        }

        public async Task<bool> DeleteRole(Guid roleId, CancellationToken ct = default)
        {
            var role = await _roleRepository.GetById(roleId, ct);
            if (role == null)
                return false;

            role.DeletedAt = DateTime.UtcNow;
            await _roleRepository.Update(role, ct);
            return true;
        }

        public async Task<RoleDto?> GetRoleById(Guid roleId, CancellationToken ct = default)
        {
            var role = await _roleRepository.GetById(roleId, ct);
            return role == null ? null : new RoleDto(role);
        }

        public async Task<RoleDto?> GetRoleByName(string roleName, CancellationToken ct = default)
        {
            var role = await _roleRepository.FirstOrDefault(r => r.Name == roleName && r.DeletedAt == null, ct);
            return role == null ? null : new RoleDto(role);
        }

        public async Task<List<RoleDto>> GetAllRoles(CancellationToken ct = default)
        {
            var roles = await _roleRepository.GetWhere(r => r.DeletedAt == null, ct);
            return roles.Select(r => new RoleDto(r)).ToList();
        }

        public async Task<PagedResponse<RoleDto>> GetRolesPaged(PagedRequest pagedRequest, CancellationToken ct = default)
        {
            int offset = pagedRequest.PageIndex * pagedRequest.PageSize;
            var roleList = (await _roleRepository.GetWhere(r => r.DeletedAt == null, ct))?.ToList() ?? new List<Role>();
            int total = roleList.Count;
            var roles = roleList
                            .Skip(offset)
                            .Take(pagedRequest.PageSize)
                            .Select(r => new RoleDto(r))
                            .ToList();
            return new PagedResponse<RoleDto>(pagedRequest.PageIndex, pagedRequest.PageSize, total, roles);
        }
    }
}
