using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QrCodeController : ControllerBase
    {
        private readonly IQrCodeService _qrCodeService;
        public QrCodeController(IQrCodeService qrCodeService) => _qrCodeService = qrCodeService;

        [HttpPost("generate")]
        public async Task<IActionResult> GenerateQrCode([FromBody] QrCodeGenerateReq request, CancellationToken ct)
        {
            try
            {
                var qrCode = await _qrCodeService.GenerateQrCode(request.AccessId, ct);
                return Ok(qrCode);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("{qrCodeId}")]
        public async Task<IActionResult> GetQrCodeById(Guid qrCodeId, CancellationToken ct)
        {
            try
            {
                var qrCode = await _qrCodeService.GetQrCodeById(qrCodeId, ct);
                return qrCode == null ? NotFound() : Ok(qrCode);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("access/{accessId}")]
        public async Task<IActionResult> GetQrCodesForAccess(Guid accessId, CancellationToken ct)
        {
            try
            {
                var qrCodes = await _qrCodeService.GetQrCodesForAccess(accessId, ct);
                return Ok(qrCodes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetQrCodesPaged([FromQuery] PagedRequest request, CancellationToken ct)
        {
            try
            {
                var pagedQrCodes = await _qrCodeService.GetQrCodesPaged(request, ct);
                return Ok(pagedQrCodes);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("validate")]
        public async Task<IActionResult> ValidateQrCode([FromQuery] Guid qrCodeId, [FromQuery] Guid scanningUserId, CancellationToken ct)
        {
            try
            {
                bool isValid = await _qrCodeService.ValidateQrCode(qrCodeId, scanningUserId, ct);
                return Ok(new { isValid });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{qrCodeId}")]
        public async Task<IActionResult> DeactivateQrCode(Guid qrCodeId, CancellationToken ct)
        {
            try
            {
                bool success = await _qrCodeService.DeactivateQrCode(qrCodeId, ct);
                return success ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
