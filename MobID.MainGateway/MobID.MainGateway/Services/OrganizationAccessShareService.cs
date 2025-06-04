using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services;

public class OrganizationAccessShareService : IOrganizationAccessShareService
{
    private readonly IGenericRepository<OrganizationAccessShare> _shareRepo;
    private readonly IGenericRepository<Access> _accessRepo;
    private readonly IGenericRepository<Organization> _orgRepo;
    private readonly IGenericRepository<OrganizationUser> _orgUserRepo;

    public OrganizationAccessShareService(
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
    // în serviciu:
    public async Task<bool> ShareAccessWithOrganizationAsync(
        AccessShareReq req,
        Guid grantedByUserId,
        CancellationToken ct = default)
    {
        // 1. Access-ul trebuie să existe
        var access = await _accessRepo.GetById(req.AccessId, ct);
        if (access is null)
            return false;

        // 2. Organizația sursă trebuie să existe
        var sourceOrg = await _orgRepo.GetById(req.FromOrganizationId, ct);
        if (sourceOrg is null)
            return false;

        // 3. Userul trebuie să fie membru activ în organizația sursă
        var isMember = await _orgUserRepo.FirstOrDefault(
            ou => ou.OrganizationId == req.FromOrganizationId
               && ou.UserId == grantedByUserId
               && ou.DeletedAt == null,
            ct) != null;
        if (!isMember)
            return false;

        // 4. Organizația țintă trebuie să existe
        var targetOrg = await _orgRepo.GetById(req.ToOrganizationId, ct);
        if (targetOrg is null)
            return false;

        // 5. Să nu existe deja un share activ
        var already = await _shareRepo.FirstOrDefault(
            s => s.AccessId == req.AccessId
              && s.SourceOrganizationId == req.FromOrganizationId
              && s.TargetOrganizationId == req.ToOrganizationId
              && s.DeletedAt == null,
            ct);
        if (already != null)
            return false;

        // 6. Creăm share-ul
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
        return true;
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

    public async Task<List<OrganizationAccessShareDto>> GetSharedAccessesBetweenOrganizationsAsync(
        Guid sourceOrgId,
        Guid targetOrgId,
        Guid userId,
        CancellationToken ct = default)
    {
        // 1. Orga sursă exista?
        var src = await _orgRepo.GetById(sourceOrgId, ct);
        if (src is null) throw new KeyNotFoundException("Source organization not found.");

        // 2. Orga țintă exista?
        var tgt = await _orgRepo.GetById(targetOrgId, ct);
        if (tgt is null) throw new KeyNotFoundException("Target organization not found.");

        // 3. Utilizatorul curent e membru în sursă?
        var isMember = await _orgUserRepo.FirstOrDefault(
            ou => ou.OrganizationId == sourceOrgId
               && ou.UserId == userId
               && ou.DeletedAt == null,
            ct) != null;
        if (!isMember)
            throw new UnauthorizedAccessException("Nu ai dreptul să vezi aceste share-uri.");

        // 4. Preluăm share-urile active
        var shares = await _shareRepo.GetWhereWithInclude(
            s => s.SourceOrganizationId == sourceOrgId
              && s.TargetOrganizationId == targetOrgId
              && s.DeletedAt == null,
            ct,
            s => s.Access,
            s => s.SourceOrganization,
            s => s.TargetOrganization
        );

        return shares
            .Select(s => new OrganizationAccessShareDto(s))
            .ToList();
    }
}