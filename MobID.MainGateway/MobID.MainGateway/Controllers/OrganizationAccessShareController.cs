using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Services.Interfaces;
using System.Security.Claims;
using MobID.MainGateway.Models.Entities;

namespace MobID.MainGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrganizationAccessShareController : ControllerBase
{
    private readonly IOrganizationAccessShareService _shareService;

    public OrganizationAccessShareController(IOrganizationAccessShareService shareService)
        => _shareService = shareService;

    /// <summary>
    /// Extrage ID-ul utilizatorului curent din JWT.
    /// </summary>
    private Guid UserId =>
        Guid.Parse(User.FindFirstValue(nameof(MobID.MainGateway.Models.Entities.User.Id)));

    /// <summary>
    /// Partajează un access de la organizația sa către o altă organizație.
    /// </summary>
    [HttpPost("grant")]
    [ProducesResponseType(typeof(OrganizationAccessShareDto), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<OrganizationAccessShareDto>> ShareAsync(
        [FromBody] AccessShareReq req,
        CancellationToken ct)
    {
        var success = await _shareService.ShareAccessWithOrganizationAsync(req, UserId, ct);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Revocă un share existent (soft-delete).
    /// </summary>
    [HttpDelete("revoke")]
    public async Task<IActionResult> RevokeAsync(
        [FromBody] AccessShareReq req,
        CancellationToken ct)
        {
        var success = await _shareService.RevokeSharedAccessAsync(req, UserId, ct);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// intoarce toate accesele shared intre 2 org
    /// </summary>
    [HttpGet("{sourceOrgId:guid}/to/{targetOrgId:guid}")]
    public async Task<IActionResult> GetSharedAccessesAsync(
       Guid sourceOrgId,
       Guid targetOrgId,
       CancellationToken ct)
    {
        try
        {
            var shares = await _shareService
                .GetSharedAccessesBetweenOrganizationsAsync(sourceOrgId, targetOrgId, UserId, ct);

            return Ok(shares);
        }
        catch (Exception)
        {
            return BadRequest();
        }
    }
}