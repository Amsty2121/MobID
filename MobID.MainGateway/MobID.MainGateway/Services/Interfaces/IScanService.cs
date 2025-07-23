using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;

namespace MobID.MainGateway.Services.Interfaces;

public interface IScanService
{
    Task<ScanDto> AddScanAsync(ScanQrReq request, CancellationToken ct = default);
    Task<ScanDto?> GetScanByIdAsync(Guid scanId, CancellationToken ct = default);
    Task<List<ScanDto>> GetScansForAccessAsync(Guid accessId, CancellationToken ct = default);
    Task<PagedResponse<ScanDto>> GetScansPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<List<ScanDto>> GetScansForUserAsync(Guid userId, CancellationToken ct = default);
    Task<List<ScanFullDto>> GetAllScansWithIncludedAsync(CancellationToken ct = default);

    Task<bool> ScanUserQr(string payload, Guid scanedByUserId, Guid orgId, Guid accessId, CancellationToken ct = default);
}
