using System.Transactions;
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
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<Organization> _orgRepo;
    private readonly IGenericRepository<AccessType> _accessTypeRepo;
    private readonly IGenericRepository<OrganizationUser> _orgUserRepo;
    private readonly IGenericRepository<QrCode> _qrCodeRepo;

    public AccessService(
        IGenericRepository<Access> accessRepo,
        IGenericRepository<Organization> orgRepo,
        IGenericRepository<AccessType> accessTypeRepo,
        IGenericRepository<OrganizationUser> orgUserRepo,
        IGenericRepository<QrCode> qrCodeRepo,
        IGenericRepository<User> userRepo)

    {
        _accessRepo = accessRepo;
        _orgRepo = orgRepo;
        _accessTypeRepo = accessTypeRepo;
        _orgUserRepo = orgUserRepo;
        _qrCodeRepo = qrCodeRepo;
        _userRepo = userRepo;
    }

    public async Task<AccessDto> CreateAccessAsync(
    AccessCreateReq req,
    Guid creatorId,
    CancellationToken ct = default)
    {
        // 0️⃣ Verificăm că user-ul există
        var user = await _userRepo.GetById(creatorId, ct)
                   ?? throw new InvalidOperationException("User not found.");

        // 1️⃣ Încarcăm AccessType (are flag-urile IsLimitedUse / IsSubscription)
        var type = await _accessTypeRepo.GetById(req.AccessTypeId, ct)
                   ?? throw new InvalidOperationException("Access type not found.");

        // 2️⃣ Validări pe bază de AccessType
        if (type.IsLimitedUse)
        {
            // similar fostului LimitedUse
            if (!req.TotalUseLimit.HasValue || req.TotalUseLimit <= 0)
                throw new InvalidOperationException("TotalUseLimit (>0) este necesar pentru acest tip de acces.");
        }
        else if (type.IsSubscription)
        {
            // similar fostului Subscription
            if (!req.SubscriptionPeriod.HasValue || req.SubscriptionPeriod.Value <= TimeSpan.Zero)
                throw new InvalidOperationException("SubscriptionPeriod (>0) este necesar pentru acest tip de acces.");
            // req.UseLimitPerPeriod poate fi null = nelimitat
        }
        else
        {
            // echivalent IdentityConfirm → forțăm 1 singură utilizare per user
            req.TotalUseLimit = 1;
        }

        // 3️⃣ Restul validărilor (organizație, permisiuni) rămâne neschimbat...

        // 4️⃣ Construim entitatea și salvăm în tranzacție
        using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);

        var access = new Access
        {
            Id = Guid.NewGuid(),
            Name = req.Name,
            Description = req.Description,
            OrganizationId = req.OrganizationId,
            AccessTypeId = type.Id,

            // limite:
            TotalUseLimit = req.TotalUseLimit,
            UseLimitPerPeriod = type.IsSubscription ? req.UseLimitPerPeriod : null,
            SubscriptionPeriod = type.IsSubscription ? req.SubscriptionPeriod : null,

            ExpirationDateTime = req.ExpirationDate,
            IsRestrictedToOrgMembers = req.RestrictToOrgMembers,
            IsRestrictedToOrganizationShare = req.RestrictToOrganizationShare,

            CreatedByUserId = creatorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _accessRepo.Add(access, ct);

        // Adăugăm QR de invitație
        var inviteQr = new QrCode
        {
            Id = Guid.NewGuid(),
            Description = $"Invitation QR for Access {access.Name}",
            Type = QrCodeType.Invite,
            AccessId = access.Id,
            ExpiresAt = access.ExpirationDateTime,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _qrCodeRepo.Add(inviteQr, ct);

        scope.Complete();
        return new AccessDto(access);
    }



    /// <inheritdoc/>
    public async Task<AccessDto?> GetAccessByIdAsync(Guid accessId, CancellationToken ct = default)
    {
        var a = await _accessRepo.GetByIdWithInclude(
            accessId, ct,
            x => x.Organization,
            x => x.CreatedByUser,
            x => x.AccessType,
            x => x.QrCodes);
        return a is null ? null : new AccessDto(a);
    }

    public async Task<bool> DeactivateAccessAsync(Guid accessId, CancellationToken ct = default)
    {
        var a = await _accessRepo.GetById(accessId, ct);
        if (a == null) return false;
        a.DeletedAt = DateTime.UtcNow;
        await _accessRepo.Update(a, ct);
        return true;
    }

    /// <inheritdoc/>
    public async Task<PagedResponse<AccessDto>> GetAccessesPagedAsync(
        PagedRequest req,
        CancellationToken ct = default)
    {
        var all = (await _accessRepo.GetWhereWithInclude(
                        x => x.DeletedAt == null, ct,
                        x => x.AccessType,
                        x => x.CreatedByUser))
                  .ToList();
        var total = all.Count;
        var items = all
            .Skip(req.PageIndex * req.PageSize)
            .Take(req.PageSize)
            .Select(x => new AccessDto(x))
            .ToList();

        return new PagedResponse<AccessDto>(req.PageIndex, req.PageSize, total, items);
    }


    /// <inheritdoc/>
    public async Task<List<QrCodeDto>> GetQrCodesForAccessAsync(Guid accessId, CancellationToken ct = default)
    {
        var qrs = await _qrCodeRepo.GetWhere(q => q.AccessId == accessId && q.DeletedAt == null, ct);
        return qrs.Select(q => new QrCodeDto(q)).ToList();
    }
}
