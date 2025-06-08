using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Entities;
using System.Security.Claims;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationAccessShareController : ControllerBase
{
    private readonly IOrganizationAccessShareService _service;

    public OrganizationAccessShareController(IOrganizationAccessShareService service)
    {
        _service = service;
    }

    private Guid UserId =>
        Guid.Parse(User.FindFirstValue(nameof(MobID.MainGateway.Models.Entities.User.Id)));

    [HttpPost]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> ShareAsync(
        [FromBody] AccessShareReq req,
        CancellationToken ct)
    {
        var result = await _service.ShareAccessWithOrganizationAsync(req, UserId, ct);
        return result ? NoContent() : BadRequest(new { message = "Access already shared." });
    }

    [HttpDelete("revoke")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RevokeAsync(
        [FromBody] AccessShareReq req,
        CancellationToken ct)
    {
        var result = await _service.RevokeSharedAccessAsync(req, ct);
        return result ? NoContent() : NotFound();
    }

    [HttpGet("{sourceOrgId:guid}/to/{targetOrgId:guid}")]
    [ProducesResponseType(typeof(List<OrganizationAccessShareDto>), 200)]
    public async Task<IActionResult> GetSharedAccessesAsync(
        Guid sourceOrgId,
        Guid targetOrgId,
        CancellationToken ct)
    {
        var list = await _service.GetSharedAccessesBetweenOrganizationsAsync(sourceOrgId, targetOrgId, ct);
        return Ok(list);
    }
}
