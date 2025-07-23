using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos.Rsp;
using MobID.MainGateway.Services.Interfaces;
using System.Security.Claims;

namespace MobID.MainGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QrCodeController : ControllerBase
{
    private readonly IQrCodeService _qrService;

    public QrCodeController(IQrCodeService qrService)
    {
        _qrService = qrService;
    }

    private Guid UserId =>
        Guid.Parse(User.FindFirstValue(nameof(MobID.MainGateway.Models.Entities.User.Id)));

    /// <summary>
    /// Generează un cod QR pentru un acces.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(QrCodeDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<QrCodeDto>> CreateQrCodeAsync(
        [FromBody] QrCodeGenerateReq req,
        CancellationToken ct)
    {

        try
        {
            var dto = await _qrService.CreateQrCodeAsync(req, ct);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Returnează un QR după ID.
    /// </summary>
    [HttpGet("{qrCodeId:guid}")]
    [ProducesResponseType(typeof(QrCodeDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<QrCodeDto>> GetQrCodeByIdAsync(
        Guid qrCodeId,
        CancellationToken ct)
    {
        var dto = await _qrService.GetQrCodeByIdAsync(qrCodeId, ct);
        return dto == null ? NotFound() : Ok(dto);
    }

    /// <summary>
    /// Listează QR-urile pentru un acces.
    /// </summary>
    [HttpGet("forAccess/{accessId:guid}")]
    [ProducesResponseType(typeof(List<QrCodeDto>), 200)]
    public async Task<ActionResult<List<QrCodeDto>>> GetQrCodesForAccessAsync(
        Guid accessId,
        CancellationToken ct)
    {
        var list = await _qrService.GetQrCodesForAccessAsync(accessId, ct);
        return Ok(list);
    }

    /// <summary>
    /// Listează toate codurile QR paginat.
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<QrCodeDto>), 200)]
    public async Task<ActionResult<PagedResponse<QrCodeDto>>> GetQrCodesPagedAsync(
        [FromQuery] PagedRequest req,
        CancellationToken ct)
    {
        var result = await _qrService.GetQrCodesPagedAsync(req, ct);
        return Ok(result);
    }

    /// <summary>
    /// Dezactivează un cod QR.
    /// </summary>
    [HttpDelete("{qrCodeId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeactivateQrCodeAsync(
        Guid qrCodeId,
        CancellationToken ct)
    {
        var success = await _qrService.DeactivateQrCodeAsync(qrCodeId, ct);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Validează un QR (pentru scanare).
    /// </summary>
    [HttpPost("{qrCodeId:guid}/validate")]
    [ProducesResponseType(typeof(AccessValidationRsp), 200)]
    public async Task<ActionResult<AccessValidationRsp>> ValidateQrCodeAsync(
        Guid qrCodeId,
        CancellationToken ct)
    {
        var rsp = await _qrService.ValidateQrCodeAsync(qrCodeId, UserId, ct);
        return Ok(rsp);
    }
}
