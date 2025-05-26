using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;

namespace MobID.MainGateway.Services.Interfaces;

public interface IScanService
{
    Task<ScanDto> AddScanAsync(ScanCreateReq request, CancellationToken ct = default);
    Task<ScanDto?> GetScanByIdAsync(Guid scanId, CancellationToken ct = default);
    Task<List<ScanDto>> GetScansForAccessAsync(Guid accessId, CancellationToken ct = default);
    Task<PagedResponse<ScanDto>> GetScansPagedAsync(PagedRequest request, CancellationToken ct = default);
}
