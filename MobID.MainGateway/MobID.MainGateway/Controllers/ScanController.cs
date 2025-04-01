using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MobID.MainGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScanController : ControllerBase
    {
        private readonly IScanService _scanService;
        public ScanController(IScanService scanService) => _scanService = scanService;

        [HttpPost("add")]
        public async Task<IActionResult> AddScan([FromBody] ScanCreateReq request, CancellationToken ct)
        {
            try
            {
                var scan = await _scanService.AddScan(request, ct);
                return Ok(scan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{scanId}")]
        public async Task<IActionResult> GetScanById(Guid scanId, CancellationToken ct)
        {
            try
            {
                var scan = await _scanService.GetScanById(scanId, ct);
                return scan == null ? NotFound() : Ok(scan);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("access/{accessId}")]
        public async Task<IActionResult> GetScansForAccess(Guid accessId, CancellationToken ct)
        {
            try
            {
                var scans = await _scanService.GetScansForAccess(accessId, ct);
                return Ok(scans);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetScansPaged([FromQuery] PagedRequest request, CancellationToken ct)
        {
            try
            {
                var pagedScans = await _scanService.GetScansPaged(request, ct);
                return Ok(pagedScans);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
