using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Enums;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services;

public class AccessService : IAccessService
{
    private readonly IGenericRepository<Access> _accessRepo;
    private readonly IGenericRepository<Organization> _orgRepo;
    private readonly IGenericRepository<AccessType> _accessTypeRepo;
    private readonly IGenericRepository<OrganizationUser> _orgUserRepo;

    public AccessService(
        IGenericRepository<Access> accessRepo,
        IGenericRepository<Organization> orgRepo,
        IGenericRepository<AccessType> accessTypeRepo,
        IGenericRepository<OrganizationUser> orgUserRepo)
    {
        _accessRepo = accessRepo;
        _orgRepo = orgRepo;
        _accessTypeRepo = accessTypeRepo;
        _orgUserRepo = orgUserRepo;
    }

    /// <summary>
    /// Creează un nou Access după validările de business.
    /// </summary>
    public async Task<AccessDto> CreateAccessAsync(
        AccessCreateReq req,
        Guid creatorId,
        CancellationToken ct = default)
    {
        // 1. Verificăm tipul de acces
        var type = await _accessTypeRepo.GetById(req.AccessTypeId, ct)
                  ?? throw new InvalidOperationException("Access type not found.");

        // 2. Validări specifice fiecărui tip
        switch (type.Name)
        {
            case "OneUse" when !req.MaxUsersPerPass.HasValue:
                throw new InvalidOperationException("MaxUsersPerPass este obligatoriu pentru OneUse.");
            case "MultiUse" when (!req.MaxUses.HasValue || !req.MaxUsersPerPass.HasValue):
                throw new InvalidOperationException("MaxUses și MaxUsersPerPass sunt obligatorii pentru MultiUse.");
            case "Subscription" when (!req.MonthlyLimit.HasValue || !req.SubscriptionPeriodMonths.HasValue):
                throw new InvalidOperationException("MonthlyLimit și SubscriptionPeriodMonths sunt obligatorii pentru Subscription.");
        }

        // 3. Verificăm existența organizației și drepturile creatorului
        var org = await _orgRepo.GetById(req.OrganizationId, ct)
                  ?? throw new InvalidOperationException("Organization not found.");
        var isMember = await _orgUserRepo
            .FirstOrDefault(ou => ou.OrganizationId == org.Id
                              && ou.UserId == creatorId
                              && ou.DeletedAt == null, ct)
            != null;
        if (!isMember)
            throw new UnauthorizedAccessException("Nu ai dreptul să creezi un Access pentru această organizație.");

        // 4. Construim entitatea și salvăm
        var access = new Access
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Description = req.Description,
            OrganizationId = org.Id,
            CreatedBy = creatorId,
            AccessTypeId = type.Id,
            ExpirationDateTime = req.ExpirationDate.HasValue
                                                ? DateTime.SpecifyKind(req.ExpirationDate.Value, DateTimeKind.Utc)
                                                : (DateTime?)null,
            RestrictToOrganizationMembers = req.RestrictToOrganizationMembers,
            ScanMode = Enum.Parse<AccessNumberScanMode>(req.ScanMode),
            MaxUses = req.MaxUses,
            MaxUsersPerPass = req.MaxUsersPerPass,
            MonthlyLimit = req.MonthlyLimit,
            SubscriptionPeriodMonths = req.SubscriptionPeriodMonths,
            UsesPerPeriod = req.UsesPerPeriod,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _accessRepo.Add(access, ct);

        return new AccessDto(access);
    }

    /// <summary>
    /// Returnează un Access după ID (include relațiile necesare).
    /// </summary>
    public async Task<AccessDto?> GetAccessByIdAsync(Guid accessId, CancellationToken ct = default)
    {
        var a = await _accessRepo.GetByIdWithInclude(
            accessId, ct,
            x => x.Organization,
            x => x.Creator,
            x => x.AccessType,
            x => x.QrCodes);
        return a is null ? null : new AccessDto(a);
    }

    /// <summary>
    /// Listează toate Acces-urile dintr-o organizație.
    /// </summary>
    public async Task<List<AccessDto>> GetAccessesForOrganizationAsync(
        Guid organizationId,
        CancellationToken ct = default)
    {
        var list = await _accessRepo.GetWhereWithInclude(
            x => x.OrganizationId == organizationId && x.DeletedAt == null,
            ct,
            x => x.Organization,
            x => x.Creator,
            x => x.AccessType,
            x => x.QrCodes);
        return list.Select(x => new AccessDto(x)).ToList();
    }

    /// <summary>
    /// Dezactivează (soft-delete) un Access.
    /// </summary>
    public async Task<bool> DeactivateAccessAsync(Guid accessId, CancellationToken ct = default)
    {
        var a = await _accessRepo.GetById(accessId, ct);
        if (a == null) return false;
        a.DeletedAt = DateTime.UtcNow;
        await _accessRepo.Update(a, ct);
        return true;
    }

    /// <summary>
    /// Paginează lista de Access-uri.
    /// </summary>
    public async Task<PagedResponse<AccessDto>> GetAccessesPagedAsync(
        PagedRequest req,
        CancellationToken ct = default)
    {
        var all = (await _accessRepo.GetWhereWithInclude(
                        x => x.DeletedAt == null, ct,
                        x => x.AccessType,
                        x => x.Creator))
                  .ToList();
        var total = all.Count;
        var items = all
            .Skip(req.PageIndex * req.PageSize)
            .Take(req.PageSize)
            .Select(x => new AccessDto(x))
            .ToList();

        return new PagedResponse<AccessDto>(req.PageIndex, req.PageSize, total, items);
    }
}