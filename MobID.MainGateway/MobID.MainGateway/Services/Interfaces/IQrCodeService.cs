using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos.Rsp;

namespace MobID.MainGateway.Services.Interfaces;

public interface IQrCodeService
{
    Task<QrCodeDto> CreateQrCodeAsync(QrCodeGenerateReq req, Guid createdByUserId, CancellationToken ct = default);
    Task<QrCodeDto?> GetQrCodeByIdAsync(Guid qrCodeId, CancellationToken ct = default);
    Task<List<QrCodeDto>> GetQrCodesForAccessAsync(Guid accessId, CancellationToken ct = default);
    Task<PagedResponse<QrCodeDto>> GetQrCodesPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<bool> DeactivateQrCodeAsync(Guid qrCodeId, CancellationToken ct = default);
    Task<AccessValidationRsp> ValidateQrCodeAsync(Guid qrCodeId, Guid scannedByUserId, CancellationToken ct = default);
}
