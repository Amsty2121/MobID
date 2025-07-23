using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Entities;
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
    private readonly IScanOrchestrationService _scanOrch;

    public ScanController(IScanService scanService, IUserAccessService userAccessService, IScanOrchestrationService scanOrch)
    {
        _scanService = scanService;
        _userAccessService = userAccessService;
        _scanOrch = scanOrch;
    }

    private Guid UserId =>
        Guid.Parse(User.FindFirstValue(nameof(MobID.MainGateway.Models.Entities.User.Id)));

    /// <summary>
    /// Înregistrează o nouă scanare (pentru QrCodeId), atribuită utilizatorului curent.
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ScanDto>> PostScanAsync(
    [FromBody] ScanQrReq req,
    CancellationToken ct)
    {
        try
        {
            var scan = await _scanOrch.HandleQrScanAsync(req.QrRawValue, UserId, ct);
            return Ok(scan);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
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

    [HttpGet("user")]
    [ProducesResponseType(typeof(List<ScanDto>), 200)]
    public async Task<ActionResult<List<ScanDto>>> GetScansForUserAsync(CancellationToken ct)
    {
        var userId = UserId; // preluat din token
        var list = await _scanService.GetScansForUserAsync(userId, ct);
        return Ok(list);
    }

    /// <summary>
    /// Listează toate scanările cu detalii complete (QR → Access → Org → useri).
    /// </summary>
    [HttpGet("full")]
    [ProducesResponseType(typeof(List<ScanFullDto>), 200)]
    public async Task<ActionResult<List<ScanFullDto>>> GetAllScansWithDetailsAsync(
        CancellationToken ct)
    {
        var list = await _scanService.GetAllScansWithIncludedAsync(ct);
        return Ok(list);
    }



    [HttpPost("by-scanner")]
    public async Task<IActionResult> AddScanByScanerAsync(
        [FromBody] ScanQRByScanerReq req,
        CancellationToken ct)
    {
        var result = await _scanService.ScanUserQr(req.Payload, UserId, req.OrganizationId, req.AccessId, ct);

        return Ok( new { success = result});  
    }
}