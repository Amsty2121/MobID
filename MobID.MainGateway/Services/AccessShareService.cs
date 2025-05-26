using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services;

public class AccessShareService : IAccessShareService
{
    private readonly IGenericRepository<OrganizationAccessShare> _shareRepo;
    private readonly IGenericRepository<Access> _accessRepo;
    private readonly IGenericRepository<Organization> _orgRepo;
    private readonly IGenericRepository<OrganizationUser> _orgUserRepo;

    public AccessShareService(
        IGenericRepository<OrganizationAccessShare> shareRepo,
        IGenericRepository<Access> accessRepo,
        IGenericRepository<Organization> orgRepo,
        IGenericRepository<OrganizationUser> orgUserRepo)
    {
        _shareRepo = shareRepo;
        _accessRepo = accessRepo;
        _orgRepo = orgRepo;
        _orgUserRepo = orgUserRepo;
    }

    /// <summary>
    /// Partajează un Access de la o organizație sursă la una țintă.
    /// </summary>
    public async Task<OrganizationAccessShareDto> ShareAccessWithOrganizationAsync(
        AccessShareReq req,
        Guid grantedByUserId,
        CancellationToken ct = default)
    {
        // 1. Access-ul există?
        var access = await _accessRepo.GetById(req.AccessId, ct)
                    ?? throw new InvalidOperationException("Access not found.");

        // 2. Verificăm că grantedByUserId e member al organizației sursă
        var isMember = await _orgUserRepo.FirstOrDefault(
            ou => ou.OrganizationId == req.FromOrganizationId
               && ou.UserId == grantedByUserId
               && ou.DeletedAt == null,
            ct) != null;
        if (!isMember)
            throw new UnauthorizedAccessException("Nu ai dreptul să partajezi acest acces.");

        // 3. Organizația țintă există?
        var targetOrg = await _orgRepo.GetById(req.ToOrganizationId, ct)
                       ?? throw new InvalidOperationException("Target organization not found.");

        // 4. Creeăm și salvăm share-ul
        var share = new OrganizationAccessShare
        {
            Id = Guid.NewGuid(),
            AccessId = req.AccessId,
            SourceOrganizationId = req.FromOrganizationId,
            TargetOrganizationId = req.ToOrganizationId,
            CreatedBy = grantedByUserId,
            CreatedAt = DateTime.UtcNow
        };
        await _shareRepo.Add(share, ct);

        return new OrganizationAccessShareDto(share);
    }

    public async Task<bool> RevokeSharedAccessAsync(Guid shareId, CancellationToken ct = default)
    {
        var share = await _shareRepo.GetById(shareId, ct);
        if (share == null) return false;
        share.DeletedAt = DateTime.UtcNow;
        await _shareRepo.Update(share, ct);
        return true;
    }

    public async Task<List<OrganizationAccessShareDto>> GetSharesForAccessAsync(
        Guid accessId,
        CancellationToken ct = default)
    {
        var shares = await _shareRepo.GetWhere(s => s.AccessId == accessId && s.DeletedAt == null, ct);
        return shares.Select(s => new OrganizationAccessShareDto(s)).ToList();
    }

    public async Task<List<OrganizationAccessShareDto>> GetSharesForOrganizationAsync(
        Guid organizationId,
        CancellationToken ct = default)
    {
        var shares = await _shareRepo.GetWhere(
            s => s.TargetOrganizationId == organizationId && s.DeletedAt == null, ct);
        return shares.Select(s => new OrganizationAccessShareDto(s)).ToList();
    }

    public async Task<bool> RevokeSharedAccessAsync(
        AccessShareReq req,
        Guid revokedByUserId,
        CancellationToken ct = default)
    {
        var share = await _shareRepo.FirstOrDefault(s =>
            s.AccessId == req.AccessId &&
            s.SourceOrganizationId == req.FromOrganizationId &&
            s.TargetOrganizationId == req.ToOrganizationId &&
            s.DeletedAt == null,
            ct
        );
        if (share == null) return false;

        var isMember = await _orgUserRepo.FirstOrDefault(
            ou => ou.OrganizationId == req.FromOrganizationId
               && ou.UserId == revokedByUserId
               && ou.DeletedAt == null,
            ct) != null;

        if (!isMember)
            throw new UnauthorizedAccessException(
                "Nu aveți dreptul să revocați acest share.");

        share.DeletedAt = DateTime.UtcNow;
        await _shareRepo.Update(share, ct);
        return true;
    }
}