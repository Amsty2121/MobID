using MobID.MainGateway.Models.Dtos;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MobID.MainGateway.Services.Interfaces
{
    public interface IQrCodeService
    {
        Task<QrCodeDto> GenerateQrCode(Guid accessId, CancellationToken ct = default);
        Task<QrCodeDto?> GetQrCodeById(Guid qrCodeId, CancellationToken ct = default);
        Task<List<QrCodeDto>> GetQrCodesForAccess(Guid accessId, CancellationToken ct = default);
        Task<bool> ValidateQrCode(Guid qrCodeId, Guid scanningUserId, CancellationToken ct = default);
        Task<bool> DeactivateQrCode(Guid qrCodeId, CancellationToken ct = default);
        Task<PagedResponse<QrCodeDto>> GetQrCodesPaged(PagedRequest pagedRequest, CancellationToken ct = default);
    }
}
