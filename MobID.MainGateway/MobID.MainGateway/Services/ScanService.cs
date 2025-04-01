using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services
{
    public class ScanService : IScanService
    {
        private readonly IGenericRepository<Scan> _scanRepository;

        public ScanService(IGenericRepository<Scan> scanRepository)
        {
            _scanRepository = scanRepository;
        }

        public async Task<ScanDto> AddScan(ScanCreateReq request, CancellationToken ct = default)
        {
            var scan = new Scan
            {
                Id = Guid.NewGuid(),
                AccessId = request.AccessId,
                ScannedById = request.ScannedById,
                QrCodeId = request.QrCodeId,
                ScannedAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _scanRepository.Add(scan, ct);
            return new ScanDto(scan);
        }

        public async Task<ScanDto?> GetScanById(Guid scanId, CancellationToken ct = default)
        {
            var scan = await _scanRepository.GetById(scanId, ct);
            return scan == null ? null : new ScanDto(scan);
        }

        public async Task<List<ScanDto>> GetScansForAccess(Guid accessId, CancellationToken ct = default)
        {
            var scans = await _scanRepository.GetWhere(s => s.AccessId == accessId && s.DeletedAt == null, ct);
            return scans.Select(s => new ScanDto(s)).ToList();
        }

        public async Task<PagedResponse<ScanDto>> GetScansPaged(PagedRequest pagedRequest, CancellationToken ct = default)
        {
            int offset = pagedRequest.PageIndex * pagedRequest.PageSize;
            var scanList = (await _scanRepository.GetWhere(s => s.DeletedAt == null, ct))?.ToList() ?? new List<Scan>();
            int total = scanList.Count;
            var scans = scanList
                            .Skip(offset)
                            .Take(pagedRequest.PageSize)
                            .Select(s => new ScanDto(s))
                            .ToList();
            return new PagedResponse<ScanDto>(pagedRequest.PageIndex, pagedRequest.PageSize, total, scans);
        }
    }
}
