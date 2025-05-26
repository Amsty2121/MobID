using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Enums;
using MobID.MainGateway.Services.Interfaces;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace MobID.MainGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ScanController : ControllerBase
{
    private readonly IScanService _scanService;
    private readonly IUserAccessService _userAccessService;

    public ScanController(IScanService scanService, IUserAccessService userAccessService)
    {
        _scanService = scanService;
        _userAccessService = userAccessService;
    }

    private Guid UserId =>
        Guid.Parse(User.FindFirstValue(nameof(MobID.MainGateway.Models.Entities.User.Id)));

    /// <summary>
    /// Înregistrează o nouă scanare (pentru QrCodeId), atribuită utilizatorului curent.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ScanDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<ScanDto>> CreateScanAsync(
        [FromBody] ScanCreateReq req,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        // Override userul care scanează cu cel curent
        req.ScannedById = UserId;

        var dto = await _scanService.AddScanAsync(req, ct);
        return CreatedAtAction(
            nameof(GetScanByIdAsync),
            new { scanId = dto.Id },
            dto
        );
    }

    /// <summary>
    /// Obține o scanare după ID.
    /// </summary>
    [HttpGet("{scanId:guid}")]
    [ProducesResponseType(typeof(ScanDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<ScanDto>> GetScanByIdAsync(
        Guid scanId,
        CancellationToken ct)
    {
        var dto = await _scanService.GetScanByIdAsync(scanId, ct);
        return dto == null ? NotFound() : Ok(dto);
    }

    /// <summary>
    /// Listează toate scanările pentru un anumit access (QR code).
    /// </summary>
    [HttpGet("access/{accessId:guid}")]
    [ProducesResponseType(typeof(List<ScanDto>), 200)]
    public async Task<ActionResult<List<ScanDto>>> GetScansForAccessAsync(
        Guid accessId,
        CancellationToken ct)
    {
        var list = await _scanService.GetScansForAccessAsync(accessId, ct);
        return Ok(list);
    }

    /// <summary>
    /// Listează scanările paginat.
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<ScanDto>), 200)]
    public async Task<ActionResult<PagedResponse<ScanDto>>> GetScansPagedAsync(
        [FromQuery] PagedRequest req,
        CancellationToken ct)
    {
        var page = await _scanService.GetScansPagedAsync(req, ct);
        return Ok(page);
    }

    /*[HttpPost]
    public async Task<IActionResult> AddScanAsync(
        [FromBody] ScanCreateReq req,
        CancellationToken ct)
    {
        // 1. Înregistrăm scanarea
        var dto = await _scanService.AddScanAsync(req, ct);

        // 2. Acordăm accesul cu Invitation
        var grantReq = new UserGrantAccessReq
        {
            AccessId = dto.QrCodeId  sau dto.AccessId dacă-l extinzi ,
            TargetUserId = req.ScannedById,
            FromOrganizationId =  poți extrage din QR/Access entitate 
        };
        await _userAccessService.GrantAccessToUserAsync(grantReq, req.ScannedById, ct, AccessGrantType.Invitation);

        return CreatedAtAction(nameof(GetScanByIdAsync), new { scanId = dto.Id }, dto);
    }*/
}