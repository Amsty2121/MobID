﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Services.Interfaces;
using System.Security.Claims;

namespace MobID.MainGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccessShareController : ControllerBase
{
    private readonly IAccessShareService _shareService;

    public AccessShareController(IAccessShareService shareService)
        => _shareService = shareService;

    /// <summary>
    /// Extrage ID-ul utilizatorului curent din JWT.
    /// </summary>
    private Guid UserId =>
        Guid.Parse(User.FindFirstValue(nameof(MobID.MainGateway.Models.Entities.User.Id)));

    /// <summary>
    /// Partajează un access de la organizația sa către o altă organizație.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrganizationAccessShareDto), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<OrganizationAccessShareDto>> ShareAsync(
        [FromBody] AccessShareReq req,
        CancellationToken ct)
    {
        var dto = await _shareService.ShareAccessWithOrganizationAsync(
            req,
            UserId,
            ct
        );

        return Ok(dto);
    }

    /// <summary>
    /// Listează toate share-urile pentru un anumit access.
    /// </summary>
    [HttpGet("{accessId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<OrganizationAccessShareDto>), 200)]
    public async Task<ActionResult<IEnumerable<OrganizationAccessShareDto>>> GetSharesForAccessAsync(
        Guid accessId,
        CancellationToken ct)
    {
        var shares = await _shareService.GetSharesForAccessAsync(accessId, ct);
        return Ok(shares);
    }

    /// <summary>
    /// Listează toate share-urile primite de o organizație.
    /// </summary>
    [HttpGet("organization/{organizationId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<OrganizationAccessShareDto>), 200)]    
    public async Task<ActionResult<IEnumerable<OrganizationAccessShareDto>>> GetSharesForOrganizationAsync(
        Guid organizationId,
        CancellationToken ct)
    {
        var shares = await _shareService.GetSharesForOrganizationAsync(organizationId, ct);
        return Ok(shares);
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
}