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
    public async Task<OrganizationAccessShareDto> ShareAccessWithOrganizationAsync(
        AccessShareReq req,
        Guid grantedByUserId,
        CancellationToken ct = default)
    {
        var access = await _accessRepo.GetById(req.AccessId, ct)
                    ?? throw new InvalidOperationException("Access not found.");

        var isMember = await _orgUserRepo.FirstOrDefault(
            ou => ou.OrganizationId == req.FromOrganizationId
               && ou.UserId == grantedByUserId
               && ou.DeletedAt == null,
            ct) != null;
        if (!isMember)
            throw new UnauthorizedAccessException("Nu ai dreptul să partajezi acest acces.");

        var targetOrg = await _orgRepo.GetById(req.ToOrganizationId, ct)
                       ?? throw new InvalidOperationException("Target organization not found.");

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