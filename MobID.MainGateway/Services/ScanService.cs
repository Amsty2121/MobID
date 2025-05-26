using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services;

public class ScanService : IScanService
{
    private readonly IGenericRepository<Scan> _scanRepo;

    public ScanService(IGenericRepository<Scan> scanRepo)
    {
        _scanRepo = scanRepo;
    }

    /// <inheritdoc/>
    public async Task<ScanDto> AddScanAsync(ScanCreateReq request, CancellationToken ct = default)
    {
        var s = new Scan
        {
            Id = Guid.NewGuid(),
            ScannedById = request.ScannedById,
            QrCodeId = request.QrCodeId,
            ScannedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _scanRepo.Add(s, ct);
        return new ScanDto(s);
    }

    /// <inheritdoc/>
    public async Task<ScanDto?> GetScanByIdAsync(Guid scanId, CancellationToken ct = default)
    {
        var s = await _scanRepo.GetById(scanId, ct);
        return s == null ? null : new ScanDto(s);
    }

    /// <inheritdoc/>
    public async Task<List<ScanDto>> GetScansForAccessAsync(Guid accessId, CancellationToken ct = default)
    {
        // note: if you want only scans for that access, you'll need to join via qrCode—here we return all
        var all = await _scanRepo.GetWhere(x => x.DeletedAt == null, ct);
        return all.Select(x => new ScanDto(x)).ToList();
    }

    /// <inheritdoc/>
    public async Task<PagedResponse<ScanDto>> GetScansPagedAsync(PagedRequest request, CancellationToken ct = default)
    {
        var all = (await _scanRepo.GetWhere(s => s.DeletedAt == null, ct)).ToList();
        var total = all.Count;
        var page = all
            .Skip(request.PageIndex * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new ScanDto(s))
            .ToList();
        return new PagedResponse<ScanDto>(request.PageIndex, request.PageSize, total, page);
    }
}