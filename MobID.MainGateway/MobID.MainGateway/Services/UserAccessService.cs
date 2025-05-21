using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;
using MobID.MainGateway.Models.Enums;

namespace MobID.MainGateway.Services;

public class UserAccessService : IUserAccessService
{
    private readonly IGenericRepository<UserAccess> _uaRepo;
    private readonly IGenericRepository<Access> _accessRepo;
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<OrganizationUser> _orgUserRepo;

    public UserAccessService(
        IGenericRepository<UserAccess> uaRepo,
        IGenericRepository<Access> accessRepo,
        IGenericRepository<User> userRepo,
        IGenericRepository<OrganizationUser> orgUserRepo)
    {
        _uaRepo = uaRepo;
        _accessRepo = accessRepo;
        _userRepo = userRepo;
        _orgUserRepo = orgUserRepo;
    }

    /// <inheritdoc/>
    public async Task<bool> GrantAccessToUserAsync(
        UserGrantAccessReq req,
        Guid grantedByUserId,
        CancellationToken ct = default,
        AccessGrantType grantType = AccessGrantType.DirectGrant)
    {
        // 1. Validări de existență
        var user   = await _userRepo.GetById(req.TargetUserId, ct)
                    ?? throw new InvalidOperationException("User not found.");
        var access = await _accessRepo.GetById(req.AccessId, ct)
                    ?? throw new InvalidOperationException("Access not found.");

        // 2. Fără duplicate
        if (await _uaRepo.FirstOrDefault(x =>
            x.UserId == req.TargetUserId &&
            x.AccessId == req.AccessId &&
            x.DeletedAt == null, ct) != null)
            return false;

        // 3. Creăm legătura cu GrantType.DirectGrant
        var ua = new UserAccess
        {
            Id               = Guid.NewGuid(),
            UserId           = req.TargetUserId,
            AccessId         = req.AccessId,
            GrantedByUserId  = grantedByUserId,
            GrantType        = grantType,
            CreatedAt        = DateTime.UtcNow
        };
        await _uaRepo.Add(ua, ct);
        return true;
    }


    /// <inheritdoc/>
    public async Task<bool> RevokeAccessFromUserAsync(
        UserGrantAccessReq req,
        Guid revokedByUserId,
        CancellationToken ct = default)
    {
        var ua = await _uaRepo.FirstOrDefault(x =>
            x.UserId == req.TargetUserId &&
            x.AccessId == req.AccessId &&
            x.DeletedAt == null, ct);
        if (ua == null) return false;

        ua.DeletedAt = DateTime.UtcNow;
        // you might wish to record revokedByUserId somewhere
        await _uaRepo.Update(ua, ct);
        return true;
    }

    /// <inheritdoc/>
    public async Task<List<AccessDto>> GetAccessesForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var relations = await _uaRepo.GetWhereWithInclude(
            ua => ua.UserId == userId && ua.DeletedAt == null,
            ct,
            ua => ua.Access,
            ua => ua.Access.Organization,
            ua => ua.Access.AccessType,
            ua => ua.Access.Creator,
            ua => ua.Access.QrCodes);

        return relations.Select(r => new AccessDto(r.Access)).ToList();
    }

    /// <inheritdoc/>
    public async Task<PagedResponse<AccessDto>> GetUserAccessesPagedAsync(
        Guid userId,
        PagedRequest request,
        CancellationToken ct = default)
    {
        var allRels = (await _uaRepo.GetWhereWithInclude(
            ua => ua.UserId == userId && ua.DeletedAt == null,
            ct,
            ua => ua.Access,
            ua => ua.Access.Organization,
            ua => ua.Access.AccessType,
            ua => ua.Access.Creator,
            ua => ua.Access.QrCodes)).ToList();

        var total = allRels.Count;
        var page = allRels
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new AccessDto(r.Access))
            .ToList();

        return new PagedResponse<AccessDto>(request.PageIndex, request.PageSize, total, page);
    }
}
