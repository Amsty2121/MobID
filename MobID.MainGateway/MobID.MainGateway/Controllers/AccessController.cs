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
public class AccessController : ControllerBase
{
    private readonly IAccessService _accessService;

    public AccessController(IAccessService accessService)
    {
        _accessService = accessService;
    }

    private Guid UserId =>
        Guid.Parse(User.FindFirstValue(nameof(MobID.MainGateway.Models.Entities.User.Id)));

    [HttpPost]
    [ProducesResponseType(typeof(AccessDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<AccessDto>> CreateAsync([FromBody] AccessCreateReq req, CancellationToken ct)
    {
        var dto = await _accessService.CreateAccessAsync(req, UserId, ct);
        return Ok(dto);
    }

    [HttpGet("{accessId}")]
    [ProducesResponseType(typeof(AccessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccessDto>> GetByIdAsync(Guid accessId, CancellationToken ct)
    {
        var dto = await _accessService.GetByIdAsync(accessId, ct);
        if (dto == null)
            return NotFound();

        return Ok(dto);
    }

    [HttpGet("organization/{organizationId}")]
    [ProducesResponseType(typeof(List<AccessDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AccessDto>>> GetByOrganizationAsync(Guid organizationId, CancellationToken ct)
    {
        var result = await _accessService.GetAccessesForOrganizationAsync(organizationId, ct);
        return Ok(result);
    }

    [HttpDelete("{accessId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> DeactivateAsync(Guid accessId, CancellationToken ct)
    {
        var updated = await _accessService.DeactivateAccessAsync(accessId, UserId, ct);
        if (updated == 0) return NotFound();

        return NoContent();
    }

    [HttpPut("{accessId:guid}")]
    [ProducesResponseType(typeof(AccessDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AccessDto>> UpdateAsync(
            [FromBody] AccessUpdateReq req,
            CancellationToken ct)
    {
        var updated = await _accessService.UpdateAccessAsync(req, UserId, ct);
        if (updated == null)
            return NotFound();

        return Ok(updated);
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(List<AccessDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<AccessDto>>> GetAllAsync(CancellationToken ct)
    {
        var list = await _accessService.GetAllAccessesAsync(ct);
        return Ok(list);
    }
}
