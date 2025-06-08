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
    private readonly IGenericRepository<AccessType> _accessTypeRepo;
    private readonly IGenericRepository<Organization> _orgRepo;
    private readonly IGenericRepository<OrganizationUser> _orgUserRepo;
    private readonly IGenericRepository<User> _userRepo;

    public AccessService(
        IGenericRepository<Access> accessRepo,
        IGenericRepository<AccessType> accessTypeRepo,
        IGenericRepository<Organization> orgRepo,
        IGenericRepository<OrganizationUser> orgUserRepo,
        IGenericRepository<User> userRepo)
    {
        _accessRepo = accessRepo;
        _accessTypeRepo = accessTypeRepo;
        _orgRepo = orgRepo;
        _orgUserRepo = orgUserRepo;
        _userRepo = userRepo;
    }

    public async Task<AccessDto> CreateAccessAsync(AccessCreateReq req, Guid userId, CancellationToken ct = default)
    {
        // 1️⃣ Verific utilizator
        var user = await _userRepo.GetById(userId, ct)
                   ?? throw new InvalidOperationException("Utilizatorul nu a fost găsit.");

        // 2️⃣ Încarc tip și organizație
        var accessType = await _accessTypeRepo.GetById(req.AccessTypeId, ct)
                          ?? throw new InvalidOperationException("Tipul de acces nu a fost găsit.");
        var org = await _orgRepo.GetById(req.OrganizationId, ct)
                  ?? throw new InvalidOperationException("Organizația nu a fost găsită.");

        // 3️⃣ Permisiuni: doar Owner sau Admin al org
        var rel = await _orgUserRepo.FirstOrDefault(
            ou => ou.OrganizationId == org.Id
               && ou.UserId == userId
               && ou.DeletedAt == null, ct);

        if (rel is null
            || (rel.Role != OrganizationUserRole.Owner
             && rel.Role != OrganizationUserRole.Admin))
        {
            throw new UnauthorizedAccessException("Nu ai dreptul să creezi acces în această organizație.");
        }

        // 4️⃣ Construiesc entitatea
        var access = new Access
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Description = req.Description,
            OrganizationId = org.Id,
            AccessTypeId = accessType.Id,
            CreatedByUserId = userId,
            UpdatedByUserId = userId,      
            ExpirationDateTime = req.ExpirationDateTime,
            RestrictToOrgMembers = req.RestrictToOrgMembers,
            RestrictToOrgSharing = req.RestrictToOrgSharing,
            IsMultiScan = req.IsMultiScan,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            DeletedAt = null
        };

        // 5️⃣ Reguli pe tip
        switch (accessType.Code)
        {
            case AccessTypeCode.OneUse:
                access.TotalUseLimit = 1;
                break;
            case AccessTypeCode.LimitedUse:
                if (req.TotalUseLimit <= 0)
                    throw new InvalidOperationException("TotalUseLimit obligatoriu pentru LimitedUse.");
                access.TotalUseLimit = req.TotalUseLimit;
                break;
            case AccessTypeCode.Subscription:
                if (req.SubscriptionPeriodMonths <= 0)
                    throw new InvalidOperationException("SubscriptionPeriodMonths obligatoriu pentru Subscription.");
                access.SubscriptionPeriodMonths = req.SubscriptionPeriodMonths;
                access.UseLimitPerPeriod = req.UseLimitPerPeriod;
                break;
            case AccessTypeCode.Unlimited:
                access.TotalUseLimit = null;
                access.SubscriptionPeriodMonths = null;
                access.UseLimitPerPeriod = null;
                break;
            default:
                throw new InvalidOperationException("AccessType necunoscut.");
        }

        await _accessRepo.Add(access, ct);
        return new AccessDto(access);
    }

    public async Task<AccessDto?> GetByIdAsync(Guid accessId, CancellationToken ct = default)
    {
        var a = await _accessRepo.FirstOrDefaultWithInclude(
            x => x.Id == accessId && x.DeletedAt == null, ct,
            x => x.Organization,
            x => x.AccessType,
            x => x.CreatedByUser,
            x => x.UpdatedByUser);

        return a is null ? null : new AccessDto(a);
    }

    public async Task<List<AccessDto>> GetAccessesForOrganizationAsync(Guid orgId, CancellationToken ct = default)
    {
        var list = await _accessRepo.GetWhereWithInclude(
            x => x.OrganizationId == orgId && x.DeletedAt == null, ct,
            x => x.Organization,
            x => x.AccessType,
            x => x.CreatedByUser,
            x => x.UpdatedByUser);

        return list.Select(a => new AccessDto(a)).ToList();
    }

    public async Task<int> DeactivateAccessAsync(Guid accessId, Guid userId, CancellationToken ct = default)
    {
        var access = await _accessRepo.GetById(accessId, ct)
                     ?? throw new InvalidOperationException("Access inexistent.");

        // permisiuni
        var rel = await _orgUserRepo.FirstOrDefault(
            ou => ou.OrganizationId == access.OrganizationId
               && ou.UserId == userId
               && ou.DeletedAt == null, ct);
        if (rel is null || (rel.Role != OrganizationUserRole.Owner && rel.Role != OrganizationUserRole.Admin))
            throw new UnauthorizedAccessException();

        access.DeletedAt = DateTime.UtcNow;
        access.UpdatedAt = DateTime.UtcNow;
        access.UpdatedByUserId = userId;
        return await _accessRepo.Update(access, ct);
    }

    public async Task<AccessDto?> UpdateAccessAsync(AccessUpdateReq req, Guid userId, CancellationToken ct = default)
    {
        var access = await _accessRepo.FirstOrDefaultWithInclude(
            x => x.Id == req.Id && x.DeletedAt == null, ct,
            x => x.Organization,
            x => x.AccessType,
            x => x.CreatedByUser);

        if (access == null) return null;

        // permisiuni
        var rel = await _orgUserRepo.FirstOrDefault(
            ou => ou.OrganizationId == access.OrganizationId
               && ou.UserId == userId
               && ou.DeletedAt == null, ct);
        if (rel is null || (rel.Role != OrganizationUserRole.Owner && rel.Role != OrganizationUserRole.Admin))
            throw new UnauthorizedAccessException();

        // update fields
        access.Name = req.Name;
        access.Description = req.Description;
        access.ExpirationDateTime = req.ExpirationDateTime;
        access.RestrictToOrgMembers = req.RestrictToOrgMembers;
        access.RestrictToOrgSharing = req.RestrictToOrgSharing;
        access.IsMultiScan = req.IsMultiScan;
        access.UpdatedByUserId = userId;

        // reguli pe tip
        switch (access.AccessType.Code)
        {
            case AccessTypeCode.OneUse:
                access.TotalUseLimit = 1;
                access.SubscriptionPeriodMonths = null;
                access.UseLimitPerPeriod = null;
                break;
            case AccessTypeCode.LimitedUse:
                if (req.TotalUseLimit <= 0)
                    throw new InvalidOperationException("TotalUseLimit obligatoriu pentru LimitedUse.");
                access.TotalUseLimit = req.TotalUseLimit;
                access.SubscriptionPeriodMonths = null;
                access.UseLimitPerPeriod = null;
                break;
            case AccessTypeCode.Subscription:
                if (req.SubscriptionPeriodMonths <= 0)
                    throw new InvalidOperationException("SubscriptionPeriodMonths obligatoriu pentru Subscription.");
                access.SubscriptionPeriodMonths = req.SubscriptionPeriodMonths;
                access.UseLimitPerPeriod = req.UseLimitPerPeriod;
                access.TotalUseLimit = null;
                break;
            case AccessTypeCode.Unlimited:
                access.TotalUseLimit = null;
                access.SubscriptionPeriodMonths = null;
                access.UseLimitPerPeriod = null;
                break;
            default:
                throw new InvalidOperationException("AccessType necunoscut.");
        }

        access.UpdatedAt = DateTime.UtcNow;
        await _accessRepo.Update(access, ct);
        return new AccessDto(access);
    }
}