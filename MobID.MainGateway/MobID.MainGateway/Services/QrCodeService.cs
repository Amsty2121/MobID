﻿using MobID.MainGateway.Models.Dtos;
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
    public async Task<QrCodeDto> CreateQrCodeAsync(Guid accessId, CancellationToken ct = default)
    {
        var access = await _accessRepo.GetById(accessId, ct)
                   ?? throw new InvalidOperationException("Access not found.");
        var qr = new QrCode { Id = Guid.NewGuid(), AccessId = accessId, CreatedAt = DateTime.UtcNow };
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
    public async Task<List<QrCodeDto>> GetQrCodesForAccessAsync(Guid accessId, CancellationToken ct = default)
    {
        var qrs = await _qrRepo.GetWhere(q => q.AccessId == accessId && q.DeletedAt == null, ct);
        return qrs.Select(q => new QrCodeDto(q)).ToList();
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

        if (access.RestrictToOrganizationMembers)
        {
            var member = await _orgUserRepo.FirstOrDefault(
                ou => ou.OrganizationId == access.OrganizationId && ou.UserId == scanningUserId, ct);
            if (member == null) return false;
        }

        return true;
    }
}
