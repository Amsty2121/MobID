using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Dtos.Rsp;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;

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
    public async Task<RoleDto> CreateRoleAsync(
    RoleCreateReq req,
    CancellationToken ct = default)
    {
        var existing = await _roleRepo.FirstOrDefault(r => r.Name == req.Name, ct);

        if (existing != null)
        {
            if (existing.DeletedAt == null)
                throw new InvalidOperationException($"Role '{req.Name}' already exists.");
            
            existing.DeletedAt = null;
            existing.Description = req.Description;
            existing.UpdatedAt = DateTime.UtcNow;
            await _roleRepo.Update(existing, ct);
            return new RoleDto(existing);
        }

        var role = new Role
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Description = req.Description,
            CreatedAt = DateTime.UtcNow
        };

        await _roleRepo.Add(role, ct);
        return new RoleDto(role);
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
        r.DeletedAt = r.UpdatedAt = DateTime.UtcNow;
        await _roleRepo.Update(r, ct);
        return true;
    }
}