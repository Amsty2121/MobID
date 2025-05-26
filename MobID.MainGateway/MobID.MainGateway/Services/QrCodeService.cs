using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services;

public class QrCodeService : IQrCodeService
{
    private readonly IGenericRepository<QrCode> _qrRepo;
    private readonly IGenericRepository<Access> _accessRepo;
    private readonly IGenericRepository<OrganizationUser> _orgUserRepo;

    public QrCodeService(
        IGenericRepository<QrCode> qrRepo,
        IGenericRepository<Access> accessRepo,
        IGenericRepository<OrganizationUser> orgUserRepo)
    {
        _qrRepo = qrRepo;
        _accessRepo = accessRepo;
        _orgUserRepo = orgUserRepo;
    }

    /// <inheritdoc/>
    public async Task<QrCodeDto> CreateQrCodeAsync(QrCodeGenerateReq req, CancellationToken ct = default)
    {
        var qr = new QrCode
        {
            Id = Guid.NewGuid(),
            AccessId = req.AccessId,
            Type = req.Type,
            Description = req.Description ?? $"QR generat la {DateTime.UtcNow:yyyy-MM-dd HH:mm}",
            ExpiresAt = req.ExpiresAt,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _qrRepo.Add(qr, ct);

        return new QrCodeDto(qr);
    }

    /// <inheritdoc/>
    public async Task<QrCodeDto?> GetQrCodeByIdAsync(Guid qrCodeId, CancellationToken ct = default)
    {
        var qr = await _qrRepo.GetById(qrCodeId, ct);
        return qr == null ? null : new QrCodeDto(qr);
    }

    /// <inheritdoc/>
    public async Task<PagedResponse<QrCodeDto>> GetQrCodesPagedAsync(PagedRequest request, CancellationToken ct = default)
    {
        var all = (await _qrRepo.GetWhere(q => q.DeletedAt == null, ct)).ToList();
        var total = all.Count;
        var page = all
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .Select(q => new QrCodeDto(q))
            .ToList();
        return new PagedResponse<QrCodeDto>(request.PageIndex, request.PageSize, total, page);
    }

    /// <inheritdoc/>
    public async Task<bool> DeactivateQrCodeAsync(Guid qrCodeId, CancellationToken ct = default)
    {
        var qr = await _qrRepo.GetById(qrCodeId, ct);
        if (qr == null) return false;
        qr.DeletedAt = DateTime.UtcNow;
        await _qrRepo.Update(qr, ct);
        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> ValidateQrCodeAsync(Guid qrCodeId, Guid scanningUserId, CancellationToken ct = default)
    {
        var qr = await _qrRepo.GetById(qrCodeId, ct);
        if (qr == null || qr.DeletedAt != null) return false;

        var access = await _accessRepo.GetById(qr.AccessId, ct);
        if (access == null || access.DeletedAt != null ||
           (access.ExpirationDateTime.HasValue && access.ExpirationDateTime < DateTime.UtcNow))
            return false;

        if (access.IsRestrictedToOrgMembers)
        {
            var member = await _orgUserRepo.FirstOrDefault(
                ou => ou.OrganizationId == access.OrganizationId && ou.UserId == scanningUserId, ct);
            if (member == null) return false;
        }

        return true;
    }
}
