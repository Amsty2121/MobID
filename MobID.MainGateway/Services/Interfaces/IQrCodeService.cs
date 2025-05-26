using MobID.MainGateway.Models.Dtos;

namespace MobID.MainGateway.Services.Interfaces;

public interface IQrCodeService
{
    Task<QrCodeDto> CreateQrCodeAsync(Guid accessId, CancellationToken ct = default);
    Task<QrCodeDto?> GetQrCodeByIdAsync(Guid qrCodeId, CancellationToken ct = default);
    Task<List<QrCodeDto>> GetQrCodesForAccessAsync(Guid accessId, CancellationToken ct = default);
    Task<PagedResponse<QrCodeDto>> GetQrCodesPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<bool> DeactivateQrCodeAsync(Guid qrCodeId, CancellationToken ct = default);
    Task<bool> ValidateQrCodeAsync(Guid qrCodeId, Guid scanningUserId, CancellationToken ct = default);
}
