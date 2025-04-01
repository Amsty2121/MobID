using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;

namespace MobID.MainGateway.Services.Interfaces
{
    public interface IScanService
    {
        Task<ScanDto> AddScan(ScanCreateReq request, CancellationToken ct = default);
        Task<ScanDto?> GetScanById(Guid scanId, CancellationToken ct = default);
        Task<List<ScanDto>> GetScansForAccess(Guid accessId, CancellationToken ct = default);
        Task<PagedResponse<ScanDto>> GetScansPaged(PagedRequest pagedRequest, CancellationToken ct = default);
    }
}
