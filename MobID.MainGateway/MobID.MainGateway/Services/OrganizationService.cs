using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Enums;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services;

public class OrganizationService : IOrganizationService
{
    private readonly IGenericRepository<Organization> _orgRepo;
    private readonly IGenericRepository<OrganizationUser> _orgUserRepo;
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<Access> _accessRepo;
    private readonly IGenericRepository<UserAccess> _uaRepo;

    public OrganizationService(
        IGenericRepository<Organization> orgRepo,
        IGenericRepository<OrganizationUser> orgUserRepo,
        IGenericRepository<User> userRepo,
        IGenericRepository<UserAccess> uaRepo,
        IGenericRepository<Access> accessRepo)
    {
        _orgRepo = orgRepo;
        _orgUserRepo = orgUserRepo;
        _userRepo = userRepo;
        _uaRepo = uaRepo;
        _accessRepo = accessRepo;
    }

    /// <inheritdoc/>
    public async Task<OrganizationDto> CreateOrganizationAsync(
        OrganizationCreateReq request,
        CancellationToken ct = default)
    {
        if (await _orgRepo.FirstOrDefault(o => o.Name == request.Name, ct) != null)
            throw new InvalidOperationException($"Organization '{request.Name}' exists.");

        var owner = await _userRepo.GetById(request.OwnerId, ct)
                    ?? throw new InvalidOperationException("Owner not found.");

        var org = new Organization
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            OwnerId = request.OwnerId,
            CreatedAt = DateTime.UtcNow
        };
        await _orgRepo.Add(org, ct);

        // add owner as member
        await AddUserToOrganizationAsync(org.Id,
            new OrganizationAddUserReq { UserId = request.OwnerId, Role = OrganizationUserRole.Owner.ToString() },
            ct);

        return new OrganizationDto(org);
    }

    /// <inheritdoc/>
    public async Task<OrganizationDto?> GetOrganizationByIdAsync(Guid organizationId, CancellationToken ct = default)
    {
        var org = await _orgRepo.GetByIdWithInclude(organizationId, ct, o => o.Owner);
        return org == null ? null : new OrganizationDto(org);
    }

    /// <inheritdoc/>
    public async Task<PagedResponse<OrganizationDto>> GetOrganizationsPagedAsync(
        PagedRequest request,
        CancellationToken ct = default)
    {
        var all = (await _orgRepo.GetWhereWithInclude(
                        o => o.DeletedAt == null, ct, o => o.Owner))
                  .ToList();
        var total = all.Count;
        var page = all
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new OrganizationDto(o))
            .ToList();

        return new PagedResponse<OrganizationDto>(
            request.PageIndex,
            request.PageSize,
            total,
            page);
    }

    /// <inheritdoc/>
    public async Task<List<OrganizationDto>> GetAllOrganizationsAsync(CancellationToken ct = default)
    {
        var list = await _orgRepo.GetWhereWithInclude(o => o.DeletedAt == null, ct, o => o.Owner);
        return list.Select(o => new OrganizationDto(o)).ToList();
    }

    /// <inheritdoc/>
    public async Task<OrganizationDto> UpdateOrganizationAsync(
        OrganizationUpdateReq request,
        CancellationToken ct = default)
    {
        var org = await _orgRepo.GetById(request.OrganizationId, ct)
                  ?? throw new InvalidOperationException("Organization not found.");

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            if (await _orgRepo.FirstOrDefault(
                    o => o.Name == request.Name && o.Id != request.OrganizationId, ct) != null)
                throw new InvalidOperationException($"Organization '{request.Name}' exists.");
            org.Name = request.Name;
        }

        org.UpdatedAt = DateTime.UtcNow;
        await _orgRepo.Update(org, ct);
        return new OrganizationDto(org);
    }

    /// <inheritdoc/>
    public async Task<bool> DeactivateOrganizationAsync(Guid organizationId, CancellationToken ct = default)
    {
        var org = await _orgRepo.GetById(organizationId, ct);
        if (org == null) return false;
        org.DeletedAt = DateTime.UtcNow;
        await _orgRepo.Update(org, ct);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> AddUserToOrganizationAsync(
        Guid organizationId,
        OrganizationAddUserReq request,
        CancellationToken ct = default)
    {
        var org = await _orgRepo.GetById(organizationId, ct)
                   ?? throw new InvalidOperationException("Organization not found.");
        var user = await _userRepo.GetById(request.UserId, ct)
                   ?? throw new InvalidOperationException("User not found.");

        if (await _orgUserRepo.FirstOrDefault(
                ou => ou.OrganizationId == organizationId && ou.UserId == request.UserId, ct) != null)
            return false;

        // parse role or default to Member
        var role = Enum.TryParse<OrganizationUserRole>(request.Role, out var r)
                       ? r
                       : OrganizationUserRole.Member;

        var orgUser = new OrganizationUser
        {
            Id = Guid.NewGuid(),
            OrganizationId = organizationId,
            UserId = request.UserId,
            Role = request.Role != null
                             ? Enum.Parse<OrganizationUserRole>(request.Role)
                             : OrganizationUserRole.Member,
            CreatedAt = DateTime.UtcNow
        };
        await _orgUserRepo.Add(orgUser, ct);

        var accesses = await _accessRepo.GetWhere(
            a => a.OrganizationId == organizationId && a.DeletedAt == null,
            ct);
        foreach (var a in accesses)
        {
            // skip duplicates
            var dup = await _uaRepo.FirstOrDefault(x =>
                x.UserId == request.UserId &&
                x.AccessId == a.Id && x.DeletedAt == null,
                ct);
            if (dup != null) continue;

            var ua = new UserAccess
            {
                Id = Guid.NewGuid(),
                UserId = request.UserId,
                AccessId = a.Id,
                GrantedByUserId = orgUser.UserId,
                GrantType = AccessGrantType.OrgMembership,
                CreatedAt = DateTime.UtcNow
            };
            await _uaRepo.Add(ua, ct);
        }

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> RemoveUserFromOrganizationAsync(
        Guid organizationId,
        Guid userId,
        CancellationToken ct = default)
    {
        var ou = await _orgUserRepo.FirstOrDefault(
            x => x.OrganizationId == organizationId && x.UserId == userId, ct);
        if (ou == null) return false;
        ou.DeletedAt = DateTime.UtcNow;
        await _orgUserRepo.Update(ou, ct);
        return true;
    }

    /// <inheritdoc/>
    public async Task<List<OrganizationUserDto>> GetUsersForOrganizationAsync(
        Guid organizationId,
        CancellationToken ct = default)
    {
        var list = await _orgUserRepo.GetWhereWithInclude(
            ou => ou.OrganizationId == organizationId && ou.DeletedAt == null,
            ct, ou => ou.User);
        return list.Select(ou => new OrganizationUserDto(ou)).ToList();
    }
}