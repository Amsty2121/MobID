using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
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
        => _qrService = qrService;

    /// <summary>
    /// Extrage ID-ul utilizatorului curent din JWT.
    /// </summary>
    private Guid UserId =>
        Guid.Parse(User.FindFirstValue(nameof(MobID.MainGateway.Models.Entities.User.Id)));

    /// <summary>
    /// Generează un cod QR pentru un access existent.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(QrCodeDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<QrCodeDto>> CreateQrCodeAsync(
        [FromBody] QrCodeGenerateReq req,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var dto = await _qrService.CreateQrCodeAsync(req.AccessId, ct);
            return CreatedAtAction(
                nameof(GetQrCodeByIdAsync),
                new { qrCodeId = dto.Id },
                dto
            );
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obține un cod QR după ID.
    /// </summary>
    [HttpGet("{qrCodeId:guid}")]
    [ProducesResponseType(typeof(QrCodeDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<QrCodeDto>> GetQrCodeByIdAsync(
        Guid qrCodeId,
        CancellationToken ct)
    {
        try
        {
            var dto = await _qrService.GetQrCodeByIdAsync(qrCodeId, ct);
            return dto == null ? NotFound() : Ok(dto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Listează toate codurile QR generate pentru un anumit access.
    /// </summary>
    [HttpGet("access/{accessId:guid}")]
    [ProducesResponseType(typeof(List<QrCodeDto>), 200)]
    public async Task<ActionResult<List<QrCodeDto>>> GetQrCodesForAccessAsync(
        Guid accessId,
        CancellationToken ct)
    {
        try
        {
            var list = await _qrService.GetQrCodesForAccessAsync(accessId, ct);
            return Ok(list);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Listează codurile QR paginat.
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<QrCodeDto>), 200)]
    public async Task<ActionResult<PagedResponse<QrCodeDto>>> GetQrCodesPagedAsync(
        [FromQuery] PagedRequest req,
        CancellationToken ct)
    {
        try
        {
            var page = await _qrService.GetQrCodesPagedAsync(req, ct);
            return Ok(page);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Validează un cod QR în contextul utilizatorului curent.
    /// </summary>
    [HttpPost("{qrCodeId:guid}/validate")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> ValidateQrCodeAsync(
        Guid qrCodeId,
        CancellationToken ct)
    {
        try
        {
            var isValid = await _qrService.ValidateQrCodeAsync(qrCodeId, UserId, ct);
            return Ok(new { isValid });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Dezactivează (soft-delete) un cod QR.
    /// </summary>
    [HttpDelete("{qrCodeId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeactivateQrCodeAsync(
        Guid qrCodeId,
        CancellationToken ct)
    {
        try
        {
            var success = await _qrService.DeactivateQrCodeAsync(qrCodeId, ct);
            return success ? NoContent() : NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}