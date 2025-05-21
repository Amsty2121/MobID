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


    /// <summary>
    /// Extracts the current user's ID from the JWT.
    /// </summary>
    private Guid UserId =>
        Guid.Parse(User.FindFirstValue(nameof(MobID.MainGateway.Models.Entities.User.Id)));

    /// <summary>
    /// Creates a new access under the current user's organization.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AccessDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<AccessDto>> CreateAsync(
        [FromBody] AccessCreateReq req,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var dto = await _accessService.CreateAccessAsync(req, UserId, ct);
        return CreatedAtAction(
            nameof(GetByIdAsync),
            new { accessId = dto.Id },
            dto
        );
    }

    /// <summary>
    /// Retrieves a single access by its ID.
    /// </summary>
    [HttpGet("{accessId:guid}")]
    [ProducesResponseType(typeof(AccessDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<AccessDto>> GetByIdAsync(
        Guid accessId,
        CancellationToken ct)
    {
        var dto = await _accessService.GetAccessByIdAsync(accessId, ct);
        if (dto == null) return NotFound();
        return Ok(dto);
    }

    /// <summary>
    /// Lists all accesses for a given organization.
    /// </summary>
    [HttpGet("organization/{organizationId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<AccessDto>), 200)]
    public async Task<ActionResult<IEnumerable<AccessDto>>> GetByOrganizationAsync(
        Guid organizationId,
        CancellationToken ct)
    {
        var list = await _accessService.GetAccessesForOrganizationAsync(organizationId, ct);
        return Ok(list);
    }

    /// <summary>
    /// Returns a paged list of all accesses.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<AccessDto>), 200)]
    public async Task<ActionResult<PagedResponse<AccessDto>>> GetPagedAsync(
        [FromQuery] PagedRequest request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var page = await _accessService.GetAccessesPagedAsync(request, ct);
        return Ok(page);
    }

    /// <summary>
    /// Soft‐deletes (deactivates) an access.
    /// </summary>
    [HttpDelete("{accessId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeleteAsync(
        Guid accessId,
        CancellationToken ct)
    {
        var ok = await _accessService.DeactivateAccessAsync(accessId, ct);
        return ok ? NoContent() : NotFound();
    }
}
