using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<UserRole> _userRoleRepo;

    public UserService(
        IGenericRepository<User> userRepo,
        IGenericRepository<UserRole> userRoleRepo)
    {
        _userRepo = userRepo;
        _userRoleRepo = userRoleRepo;
    }

    /// <inheritdoc/>
    public async Task<UserDto> CreateUserAsync(UserAddReq request, CancellationToken ct = default)
    {
        var existing = await _userRepo.FirstOrDefault(
            u => u.Email == request.Email && u.DeletedAt == null,
            ct);
        if (existing != null)
            throw new InvalidOperationException($"User with email '{request.Email}' already exist.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _userRepo.Add(user, ct);

        var defaultRoleId = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc");
        var userRole = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            RoleId = defaultRoleId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _userRoleRepo.Add(userRole, ct);

        return new UserDto(user, new[] { "SimpleUser" });
    }

    /// <inheritdoc/>
    public async Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        var u = await _userRepo.GetById(userId, ct);
        return u == null
            ? null
            : new UserDto(u, (await _userRoleRepo.GetWhereWithInclude(r => r.UserId == u.Id, ct, r => r.Role))
                               .Select(r => r.Role.Name)
                               .ToList());
    }

    /// <inheritdoc/>
    public async Task<PagedResponse<UserDto>> GetUsersPagedAsync(PagedRequest request, CancellationToken ct = default)
    {
        var all = (await _userRepo.GetWhere(u => u.DeletedAt == null, ct)).ToList();
        var total = all.Count;
        var pageEntities = all.Skip(request.PageIndex * request.PageSize)
                              .Take(request.PageSize)
                              .ToList();

        var dtos = new List<UserDto>();
        foreach (var u in pageEntities)
        {
            var roles = await _userRoleRepo.GetWhereWithInclude(r => r.UserId == u.Id, ct, r => r.Role);
            dtos.Add(new UserDto(u, roles.Where(ur => ur.Role.DeletedAt == null).Select(r => r.Role.Name).ToList()));
        }

        return new PagedResponse<UserDto>(request.PageIndex, request.PageSize, total, dtos);
    }

    /// <inheritdoc/>
    public async Task<bool> DeactivateUserAsync(Guid userId, CancellationToken ct = default)
    {
        var u = await _userRepo.GetById(userId, ct);
        if (u == null) return false;
        u.DeletedAt = DateTime.UtcNow;
        await _userRepo.Update(u, ct);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> AssignRoleToUserAsync(Guid userId, Guid roleId, CancellationToken ct = default)
    {
        if (!await _userRepo.IsIdPresent(userId)) return false;
        if (await _userRoleRepo.FirstOrDefault(r => r.UserId == userId && r.RoleId == roleId, ct) != null)
            return false;

        var ur = new UserRole
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            RoleId = roleId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
        await _userRoleRepo.Add(ur, ct);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveRoleFromUserAsync(Guid userId, Guid roleId, CancellationToken ct = default)
    {
        var ur = await _userRoleRepo.FirstOrDefault(r => r.UserId == userId && r.RoleId == roleId, ct);
        if (ur == null) return false;
        await _userRoleRepo.Remove(ur, ct);
        return true;
    }

    /// <inheritdoc/>
    public async Task<List<string>> GetUserRolesAsync(Guid userId, CancellationToken ct = default)
    {
        if (!await _userRepo.IsIdPresent(userId)) return new List<string>();
        var roles = await _userRoleRepo.GetWhereWithInclude(r => r.UserId == userId, ct, r => r.Role);
        return roles.Where(ur => ur.Role.DeletedAt == null).Select(r => r.Role.Name).ToList();
    }


}