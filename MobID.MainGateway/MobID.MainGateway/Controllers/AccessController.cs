using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessController : ControllerBase
    {
        private readonly IAccessService _accessService;
        public AccessController(IAccessService accessService)
        {
            _accessService = accessService;
        }

        private Guid UserId => Guid.Parse(HttpContext.User.Claims.First(t => t.Type == nameof(MobID.MainGateway.Models.Entities.User.Id)).Value);


        [HttpPost("create")]
        public async Task<IActionResult> CreateAccess([FromBody] AccessCreateReq request, CancellationToken ct)
        {
            try
            {
                var access = await _accessService.CreateAccess(request, UserId, ct);
                return Ok(access);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{accessId}")]
        public async Task<IActionResult> GetAccessById(Guid accessId, CancellationToken ct)
        {
            try
            {
                var access = await _accessService.GetAccessById(accessId, ct);
                return access == null ? NotFound() : Ok(access);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("organization/{organizationId}")]
        public async Task<IActionResult> GetAccessesForOrganization(Guid organizationId, CancellationToken ct)
        {
            try
            {
                var accesses = await _accessService.GetAccessesForOrganization(organizationId, ct);
                return Ok(accesses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetAccessesPaged([FromQuery] PagedRequest request, CancellationToken ct)
        {
            try
            {
                var pagedAccesses = await _accessService.GetAccessesPaged(request, ct);
                return Ok(pagedAccesses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{accessId}")]
        public async Task<IActionResult> DeactivateAccess(Guid accessId, CancellationToken ct)
        {
            try
            {
                bool success = await _accessService.DeactivateAccess(accessId, ct);
                return success ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
