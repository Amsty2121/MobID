using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;

public class OrganizationAccessShareService : IOrganizationAccessShareService
{
    private readonly IGenericRepository<OrganizationAccessShare> _repo;
    private readonly IGenericRepository<Access> _accessRepo;
    private readonly IGenericRepository<Organization> _orgRepo;

    public OrganizationAccessShareService(
        IGenericRepository<OrganizationAccessShare> repo,
        IGenericRepository<Access> accessRepo,
        IGenericRepository<Organization> orgRepo)
    {
        _repo = repo;
        _accessRepo = accessRepo;
        _orgRepo = orgRepo;
    }

    public async Task<bool> ShareAccessWithOrganizationAsync(AccessShareReq req, Guid userId, CancellationToken ct = default)
    {
        // Încarcă accesul
        var access = await _accessRepo.GetById(req.AccessId, ct)
            ?? throw new InvalidOperationException("Access not found.");

        // Regula nouă: dacă partajarea este restricționată, nu permitem sharing-ul
        if (access.RestrictToOrgSharing)
            throw new InvalidOperationException("Partajarea accesului către alte organizații nu este permisă pentru acest acces.");

        // Încarcă organizațiile sursă și destinație
        var sourceOrg = await _orgRepo.GetById(req.SourceOrganizationId, ct)
            ?? throw new InvalidOperationException("Source organization not found.");

        var targetOrg = await _orgRepo.GetById(req.TargetOrganizationId, ct)
            ?? throw new InvalidOperationException("Target organization not found.");

        // Verifică duplicatul
        var exists = await _repo.FirstOrDefault(
            s => s.AccessId == req.AccessId &&
                 s.SourceOrganizationId == req.SourceOrganizationId &&
                 s.TargetOrganizationId == req.TargetOrganizationId &&
                 s.DeletedAt == null,
            ct);

        if (exists != null)
            return false;

        // Creează partajarea
        var share = new OrganizationAccessShare
        {
            Id = Guid.NewGuid(),
            AccessId = req.AccessId,
            SourceOrganizationId = req.SourceOrganizationId,
            TargetOrganizationId = req.TargetOrganizationId,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _repo.Add(share, ct);
        return true;
    }

    public async Task<bool> RevokeSharedAccessAsync(AccessShareReq req, CancellationToken ct = default)
    {
        var share = await _repo.FirstOrDefault(
            s => s.AccessId == req.AccessId &&
                 s.SourceOrganizationId == req.SourceOrganizationId &&
                 s.TargetOrganizationId == req.TargetOrganizationId &&
                 s.DeletedAt == null,
            ct);

        if (share == null) return false;

        share.DeletedAt = DateTime.UtcNow;
        await _repo.Update(share, ct);
        return true;
    }

    public async Task<List<OrganizationAccessShareDto>> GetAccessesSharedToOrganizationAsync(Guid organizationId, CancellationToken ct = default)
    {
        var shares = await _repo.GetWhereWithInclude(
            s => s.TargetOrganizationId == organizationId && s.DeletedAt == null,
            ct,
            s => s.Access,
            s => s.SourceOrganization,
            s => s.TargetOrganization,
            s => s.Creator
        );
        return shares.Select(s => new OrganizationAccessShareDto(s)).ToList();
    }

    public async Task<List<OrganizationAccessShareDto>> GetSharedAccessesBetweenOrganizationsAsync(Guid sourceOrgId, Guid targetOrgId, CancellationToken ct = default)
    {
        var shares = await _repo.GetWhereWithInclude(
            s => s.SourceOrganizationId == sourceOrgId &&
                 s.TargetOrganizationId == targetOrgId &&
                 s.DeletedAt == null,
            ct,
            s => s.Access,
            s => s.SourceOrganization,
            s => s.TargetOrganization,
            s => s.Creator
        );
        return shares.Select(s => new OrganizationAccessShareDto(s)).ToList();
    }
}
