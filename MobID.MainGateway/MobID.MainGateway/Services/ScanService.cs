using Microsoft.AspNetCore.Mvc.Abstractions;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Enums;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;
using System.Runtime.CompilerServices;
using System.Text;

namespace MobID.MainGateway.Services;

public class ScanService : IScanService
{
    private readonly IGenericRepository<Scan> _scanRepo;
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<Organization> _orgRepo;
    private readonly IGenericRepository<OrganizationUser> _orgUserRepo;
    private readonly IGenericRepository<Access> _accessRepo;
    private readonly IGenericRepository<UserAccess> _userAccessRepo;
    private readonly IGenericRepository<OrganizationAccessShare> _orgAccessShare;

    public ScanService(IGenericRepository<Scan> scanRepo, IGenericRepository<User> userRepo, IGenericRepository<Organization> orgRepo, IGenericRepository<OrganizationUser> orgUserRepo, IGenericRepository<Access> accessRepo, IGenericRepository<UserAccess> userAccessRepo, IGenericRepository<OrganizationAccessShare> orgAccessShare)
    {
        _scanRepo = scanRepo;
        _userRepo = userRepo;
        _orgRepo = orgRepo;
        _orgUserRepo = orgUserRepo;
        _accessRepo = accessRepo;
        _userAccessRepo = userAccessRepo;
        _orgAccessShare = orgAccessShare;
    }

    /// <inheritdoc/>//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    public async Task<ScanDto> AddScanAsync(ScanQrReq request, CancellationToken ct = default)
    {
        var s = new Scan
        {
            Id = Guid.NewGuid(),
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

    public async Task<List<ScanDto>> GetScansForUserAsync(Guid userId, CancellationToken ct = default)
    {
        // luăm doar scanările active făcute de acest user
        var scans = await _scanRepo.GetWhereWithInclude(
            s => s.DeletedAt == null && s.ScannedForUserId == userId,
            ct,
            include => include.ScannedByUser, include => include.ScannedForUser
        );
        return scans.Select(s => new ScanDto(s)).ToList();
    }
    public async Task<List<ScanFullDto>> GetAllScansWithIncludedAsync(CancellationToken ct = default)
    {
        // include QR → Access → Organization, plus ScannedByUser & ScannedForUser
        var scans = await _scanRepo.GetWhereWithInclude(
            s => s.DeletedAt == null,
            ct,
            include => include.QrCode!,
            include => include.QrCode!.Access!,
            include => include.QrCode!.Access!.Organization!,
            include => include.ScannedByUser!,
            include => include.ScannedForUser!
        );

        return scans.Select(s => new ScanFullDto(s)).ToList();
    }

    public async Task<bool> ScanUserQr(
    string payload,
    Guid scannedByUserId,
    Guid orgId,
    Guid accessId,
    CancellationToken ct = default)
    {
        // 1. Decode payload din Base64
        string decoded;
        try
        {
            decoded = Encoding.UTF8.GetString(Convert.FromBase64String(payload));
        }
        catch
        {
            // dacă decode eșuează, nu putem nici măcar extrage userId, așa că retur fără scan
            return false;
        }

        // 2. Extragem părțile și validăm formatul
        var parts = decoded.Split(':');
        if (parts.Length < 2 || parts[1] != "USERQR" || !Guid.TryParse(parts[0], out var scannedForUserId))
        {
            return false;
        }

        // 3. Încarcăm utilizatorul
        var user = await _userRepo.GetByIdWithInclude(
            scannedForUserId, ct,
            u => u.UserAccesses,
            u => u.OrganizationUsers);
        if (user == null)
        {
            // user necunoscut, retur fals (fără înregistrare de scan)
            return false;
        }

        // 4. Determinăm dacă scanul este valid
        var userAccesses = await GetAllUserAccessesAsync(scannedForUserId);
        var hasAccess = userAccesses.Select(x => x.Id).Contains(accessId);


        var access = await _accessRepo.GetByIdWithInclude(accessId, ct, x => x.QrCodes);
        var qrCode = access.QrCodes.FirstOrDefault(x => x.Type == QrCodeType.AccessConfirm);
        // 5. Înregistrăm SCANUL, cu succes sau eșec
        var scan = new Scan
        {
            Id = Guid.NewGuid(),
            ScannedByUserId = scannedByUserId,
            ScannedForUserId = scannedForUserId,
            QrCodeId = qrCode.Id,    // temporar folosim accessId
            IsSuccessfull = hasAccess,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        await _scanRepo.Add(scan, ct);

        // 6. Returnăm rezultatul validării
        return hasAccess;
    }

    public async Task<List<Access>> GetAllUserAccessesAsync(Guid userId, CancellationToken ct = default)
    {
        // 1️⃣ Direct user‐assigned accesses
        var userAccesses = await _userAccessRepo.GetWhereWithInclude(
            ua => ua.UserId == userId && ua.DeletedAt == null,
            ct,
            ua => ua.Access,            // include the Access navigation
            ua => ua.Access.AccessType,
            ua => ua.Access.Organization,
            ua => ua.Access.CreatedByUser,
            ua => ua.Access.QrCodes
        );
        var direct = userAccesses.Select(ua => ua.Access);

        var memberships = await _orgUserRepo.GetWhere(
            ou => ou.UserId == userId && ou.DeletedAt == null,
            ct
        );
        var orgIds = memberships.Select(ou => ou.OrganizationId).Distinct().ToList();

        var orgAccesses = orgIds.Any()
            ? await _accessRepo.GetWhereWithInclude(
                  a => orgIds.Contains(a.OrganizationId) && a.DeletedAt == null,
                  ct,
                  a => a.AccessType,
                  a => a.Organization,
                  a => a.CreatedByUser,
                  a => a.QrCodes
              )
            : new List<Access>();

        var shares = await _orgAccessShare.GetWhere(
            s => orgIds.Contains(s.TargetOrganizationId) && s.DeletedAt == null,
            ct
        );
        var sharedIds = shares.Select(s => s.AccessId).Distinct().ToList();
        var shared = sharedIds.Any()
            ? await _accessRepo.GetWhereWithInclude(
                  a => sharedIds.Contains(a.Id) && a.DeletedAt == null,
                  ct,
                  a => a.AccessType,
                  a => a.Organization,
                  a => a.CreatedByUser,
                  a => a.QrCodes
              )
            : new List<Access>();

        var all = direct
            .Union(orgAccesses)
            .Union(shared)
            .GroupBy(a => a.Id)
            .Select(g => g.First());

        return all.ToList();
    }



}
