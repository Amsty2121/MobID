using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Dtos.Rsp;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MobID.MainGateway.Services
{
    public class RoleService : IRoleService
    {
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<UserRole> _userRoleRepository;

        public RoleService(
            IGenericRepository<Role> roleRepository,
            IGenericRepository<UserRole> userRoleRepository)
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
            if (role == null) return false;

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

        public async Task<bool> AssignRoleToUser(Guid userId, Guid roleId, CancellationToken ct = default)
        {
            var existing = await _userRoleRepository.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId, ct);
            if (existing != null) return false; // User already has this role

            var userRole = new UserRole
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                RoleId = roleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRoleRepository.Add(userRole, ct);
            return true;
        }

        public async Task<bool> RemoveRoleFromUser(Guid userId, Guid roleId, CancellationToken ct = default)
        {
            var userRole = await _userRoleRepository.FirstOrDefault(ur => ur.UserId == userId && ur.RoleId == roleId, ct);
            if (userRole == null) return false;

            await _userRoleRepository.Remove(userRole, ct);
            return true;
        }

        public async Task<List<string>> GetUserRoles(Guid userId, CancellationToken ct = default)
        {
            var roles = await _userRoleRepository.GetWhereWithInclude(ur => ur.UserId == userId, ct, ur => ur.Role);
            return roles.Select(r => r.Role.Name).ToList();
        }
    }
}
