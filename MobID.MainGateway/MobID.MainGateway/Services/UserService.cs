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
    private readonly IGenericRepository<UserAccess> _uaRepo;
    private readonly IGenericRepository<OrganizationUser> _orgUserRepo;
    private readonly IGenericRepository<Access> _accessRepo;
    private readonly IGenericRepository<OrganizationAccessShare> _shareRepo;

    public UserService(
        IGenericRepository<User> userRepo,
        IGenericRepository<UserRole> userRoleRepo,
        IGenericRepository<UserAccess> uaRepo,
        IGenericRepository<OrganizationUser> orgUserRepo,
        IGenericRepository<Access> accessRepo,
        IGenericRepository<OrganizationAccessShare> shareRepo)
    {
        _userRepo = userRepo;
        _userRoleRepo = userRoleRepo;
        _uaRepo = uaRepo;
        _orgUserRepo = orgUserRepo;
        _accessRepo = accessRepo;
        _shareRepo = shareRepo;
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

    public async Task<List<AccessDto>> GetAllUserAccessesAsync(Guid userId, CancellationToken ct = default)
    {
        // 1️⃣ Direct user‐assigned accesses
        var userAccesses = await _uaRepo.GetWhereWithInclude(
            ua => ua.UserId == userId && ua.DeletedAt == null,
            ct,
            ua => ua.Access,            // include the Access navigation
            ua => ua.Access.AccessType,
            ua => ua.Access.Organization,
            ua => ua.Access.CreatedByUser,
            ua => ua.Access.QrCodes
        );
        var direct = userAccesses.Select(ua => ua.Access);

        // 2️⃣ All orgs the user belongs to
        var memberships = await _orgUserRepo.GetWhere(
            ou => ou.UserId == userId && ou.DeletedAt == null,
            ct
        );
        var orgIds = memberships.Select(ou => ou.OrganizationId).Distinct().ToList();

        // 3️⃣ All accesses belonging to those orgs
        var orgAccesses = orgIds.Any()
            ? await _accessRepo.GetWhereWithInclude(
                  a => orgIds.Contains(a.OrganizationId) && a.DeletedAt == null,
                  ct,
                  a => a.AccessType,
                  a => a.Organization,
                  a => a.CreatedByUser,
                  a => a.QrCodes
              )
            : new List<Access>();

        // 4️⃣ All shares *to* those orgs
        var shares = await _shareRepo.GetWhere(
            s => orgIds.Contains(s.TargetOrganizationId) && s.DeletedAt == null,
            ct
        );
        var sharedIds = shares.Select(s => s.AccessId).Distinct().ToList();
        var shared = sharedIds.Any()
            ? await _accessRepo.GetWhereWithInclude(
                  a => sharedIds.Contains(a.Id) && a.DeletedAt == null,
                  ct,
                  a => a.AccessType,
                  a => a.Organization,
                  a => a.CreatedByUser,
                  a => a.QrCodes
              )
            : new List<Access>();

        // 5️⃣ Union and de-duplicate
        var all = direct
            .Union(orgAccesses)
            .Union(shared)
            .GroupBy(a => a.Id)
            .Select(g => g.First());

        // 6️⃣ Project into DTO
        return all.Select(a => new AccessDto(a)).ToList();
    }

    public async Task<List<OrganizationDto>> GetUserOrganizationsAsync(Guid userId, CancellationToken ct = default)
    {
        // find all OrganizationUser entries for this user
        var rels = await _orgUserRepo.GetWhereWithInclude(
            ou => ou.UserId == userId && ou.DeletedAt == null,
            ct,
            ou => ou.Organization
        );

        // project to OrganizationDto
        return rels
            .Select(ou => new OrganizationDto(ou.Organization))
            .ToList();
    }


}