using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Enums;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;
using System.Transactions;

namespace MobID.MainGateway.Services;

public class AccessService : IAccessService
{
    private readonly IGenericRepository<Access> _accessRepo;
    private readonly IGenericRepository<QrCode> _qrRepo;
    private readonly IGenericRepository<AccessType> _accessTypeRepo;
    private readonly IGenericRepository<Organization> _orgRepo;
    private readonly IGenericRepository<OrganizationUser> _orgUserRepo;
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<UserRole> _userRoleRepo;
    private readonly IGenericRepository<Role> _roleRepo;

    public AccessService(
        IGenericRepository<Access> accessRepo,
        IGenericRepository<AccessType> accessTypeRepo,
        IGenericRepository<Organization> orgRepo,
        IGenericRepository<OrganizationUser> orgUserRepo,
        IGenericRepository<User> userRepo,
        IGenericRepository<QrCode> qrRepo,
        IGenericRepository<UserRole> userRoleRepo,
        IGenericRepository<Role> roleRepo)
    {
        _accessRepo = accessRepo;
        _accessTypeRepo = accessTypeRepo;
        _orgRepo = orgRepo;
        _orgUserRepo = orgUserRepo;
        _userRepo = userRepo;
        _qrRepo = qrRepo;
        _userRoleRepo = userRoleRepo;
        _roleRepo = roleRepo;
    }

    public async Task<AccessDto> CreateAccessAsync(AccessCreateReq req, Guid userId, CancellationToken ct = default)
    {
        // 1) Verificăm utilizatorul
        var user = await _userRepo.GetById(userId, ct)
                   ?? throw new InvalidOperationException("Utilizatorul nu a fost găsit.");

        // Luăm rolurile globale ale user‐ului
        var userRoles = await _userRoleRepo
            .GetWhereWithInclude(ur => ur.UserId == userId, ct, ur => ur.Role);
        var isGlobalAdmin = userRoles
            .Any(ur => string.Equals(ur.Role.Name, "Admin", StringComparison.OrdinalIgnoreCase));

        // 2) Verificăm organizația
        var org = await _orgRepo.GetById(req.OrganizationId, ct)
                  ?? throw new InvalidOperationException("Organizația nu a fost găsită.");

        // 3) Verificăm tipul de acces
        var accessType = await _accessTypeRepo.GetById(req.AccessTypeId, ct)
                          ?? throw new InvalidOperationException("Tipul de acces nu a fost găsit.");

        // 4) Permisiuni: doar Owner sau Admin în organizație, sau Admin global
        var rel = await _orgUserRepo.FirstOrDefault(
            ou => ou.OrganizationId == org.Id
               && ou.UserId == userId
               && ou.DeletedAt == null,
            ct);

        var isOrgAdminOrOwner = rel != null
            && (rel.Role == OrganizationUserRole.Owner
             || rel.Role == OrganizationUserRole.Admin);

        if (!isOrgAdminOrOwner && !isGlobalAdmin)
            throw new UnauthorizedAccessException(
                "Nu ai dreptul să creezi acces în această organizație.");

        // 5) Construim entitatea Access
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
        };

        // Aplicăm reguli specifice tipului
        switch (accessType.Code)
        {
            case AccessTypeCode.OneUse:
                access.TotalUseLimit = 1;
                break;
            case AccessTypeCode.LimitedUse:
                if (req.TotalUseLimit is null || req.TotalUseLimit <= 0)
                    throw new InvalidOperationException("TotalUseLimit obligatoriu pentru LimitedUse.");
                access.TotalUseLimit = req.TotalUseLimit;
                break;
            case AccessTypeCode.Subscription:
                if (req.SubscriptionPeriodMonths is null || req.SubscriptionPeriodMonths <= 0)
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

        // 6) Salvăm access + generăm 2 QR coduri într-o tranzacție
        using var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        await _accessRepo.Add(access, ct);

        // a) QR de partajare (ShareAccess)
        var qrShare = new QrCode
        {
            Id = Guid.NewGuid(),
            AccessId = access.Id,
            Type = QrCodeType.ShareAccess,
            Description = $"QR-share pentru {access.Name}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _qrRepo.Add(qrShare, ct);

        // b) QR de confirmare acces (AccessConfirm)
        var qrConfirm = new QrCode
        {
            Id = Guid.NewGuid(),
            AccessId = access.Id,
            Type = QrCodeType.AccessConfirm,
            Description = $"QR-confirm pentru {access.Name}",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _qrRepo.Add(qrConfirm, ct);

        tx.Complete();

        return new AccessDto(access);
    }

    public async Task<AccessDto?> UpdateAccessAsync(AccessUpdateReq req, Guid userId, CancellationToken ct = default)
    {
        // 1) Verificăm utilizatorul
        var user = await _userRepo.GetByIdWithInclude(userId, ct, x=> x.UserRoles)
                   ?? throw new InvalidOperationException("Utilizatorul nu a fost găsit.");



        
        var access = await _accessRepo.FirstOrDefaultWithInclude(
            x => x.Id == req.Id && x.DeletedAt == null, ct,
            x => x.Organization,
            x => x.AccessType,
            x => x.CreatedByUser);
        if (access is null) return null;


        var roles = await _roleRepo.GetWhere(x => user.UserRoles.Select(x => x.RoleId).Contains(x.Id));

        if (!roles.Select(x => x.Name).Contains("Admin"))
        {
            var rel = await _orgUserRepo.FirstOrDefault(
            ou => ou.OrganizationId == access.OrganizationId
               && ou.UserId == userId
               && ou.DeletedAt == null, ct);
            if (rel is null || (rel.Role != OrganizationUserRole.Owner && rel.Role != OrganizationUserRole.Admin))
                throw new UnauthorizedAccessException();
        }

        // 4) Update câmpuri
        access.Name = req.Name;
        access.Description = req.Description;
        access.ExpirationDateTime = req.ExpirationDateTime;
        access.RestrictToOrgMembers = req.RestrictToOrgMembers;
        access.RestrictToOrgSharing = req.RestrictToOrgSharing;
        access.IsMultiScan = req.IsMultiScan;
        access.UpdatedByUserId = userId;

        // 5) Regulile pe tip
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

        // 6) Salvăm într-un TransactionScope (doar update-ul este necesar aici)
        using var tx = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
        access.UpdatedAt = DateTime.UtcNow;
        await _accessRepo.Update(access, ct);
        tx.Complete();

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

    public async Task<List<AccessDto>> GetAllAccessesAsync(CancellationToken ct = default)
    {
        var accesses = await _accessRepo.GetAllWithInclude(ct, 
                                                            a => a.Organization, 
                                                            a => a.CreatedByUser, 
                                                            a => a.UpdatedByUser, 
                                                            a => a.AccessType);

        return accesses.Select(a => new AccessDto(a)).ToList();
    }
}