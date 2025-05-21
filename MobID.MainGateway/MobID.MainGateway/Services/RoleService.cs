using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Dtos.Rsp;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;
using MobID.MainGateway.Models.Dtos;

namespace MobID.MainGateway.Services;

public class RoleService : IRoleService
{
    private readonly IGenericRepository<Role> _roleRepo;
    private readonly IGenericRepository<UserRole> _userRoleRepo;

    public RoleService(
        IGenericRepository<Role> roleRepo,
        IGenericRepository<UserRole> userRoleRepo)
    {
        _roleRepo = roleRepo;
        _userRoleRepo = userRoleRepo;
    }

    /// <inheritdoc/>
    public async Task<RoleDto> CreateRoleAsync(string name, string description, CancellationToken ct = default)
    {
        var r = new Role { Id = Guid.NewGuid(), Name = name, Description = description, CreatedAt = DateTime.UtcNow };
        await _roleRepo.Add(r, ct);
        return new RoleDto(r);
    }

    /// <inheritdoc/>
    public async Task<RoleDto?> GetRoleByIdAsync(Guid roleId, CancellationToken ct = default)
    {
        var r = await _roleRepo.GetById(roleId, ct);
        return r == null ? null : new RoleDto(r);
    }

    /// <inheritdoc/>
    public async Task<RoleDto?> GetRoleByNameAsync(string roleName, CancellationToken ct = default)
    {
        var r = await _roleRepo.FirstOrDefault(x => x.Name == roleName && x.DeletedAt == null, ct);
        return r == null ? null : new RoleDto(r);
    }

    /// <inheritdoc/>
    public async Task<List<RoleDto>> GetAllRolesAsync(CancellationToken ct = default)
    {
        var all = await _roleRepo.GetWhere(x => x.DeletedAt == null, ct);
        return all.Select(x => new RoleDto(x)).ToList();
    }

    /// <inheritdoc/>
    public async Task<PagedResponse<RoleDto>> GetRolesPagedAsync(PagedRequest request, CancellationToken ct = default)
    {
        var all = (await _roleRepo.GetWhere(r => r.DeletedAt == null, ct)).ToList();
        var total = all.Count;
        var page = all
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new RoleDto(r))
            .ToList();
        return new PagedResponse<RoleDto>(request.PageIndex, request.PageSize, total, page);
    }

    /// <inheritdoc/>
    public async Task<bool> DeactivateRoleAsync(Guid roleId, CancellationToken ct = default)
    {
        var r = await _roleRepo.GetById(roleId, ct);
        if (r == null) return false;
        r.DeletedAt = DateTime.UtcNow;
        await _roleRepo.Update(r, ct);
        return true;
    }
}