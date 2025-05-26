using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Services.Interfaces;
using System.Security.Claims;

namespace MobID.MainGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserAccessController : ControllerBase
{
    private readonly IUserAccessService _userAccessService;

    public UserAccessController(IUserAccessService userAccessService)
        => _userAccessService = userAccessService;

    /// <summary>
    /// Extrage ID-ul utilizatorului curent din JWT.
    /// </summary>
    private Guid UserId =>
        Guid.Parse(User.FindFirstValue(nameof(MobID.MainGateway.Models.Entities.User.Id)));

    /// <summary>
    /// Atribuie un access unui utilizator.
    /// </summary>
    [HttpPost("grant")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> GrantAccessAsync(
        [FromBody] UserGrantAccessReq req,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        var success = await _userAccessService.GrantAccessToUserAsync(req, UserId, ct);
        if (!success)
            return BadRequest(new { message = "Nu s-a putut atribui access-ul (poate a fost deja atribuit sau date invalide)." });

        return NoContent();
    }

    /// <summary>
    /// Revocă un access de la un utilizator.
    /// </summary>
    [HttpPost("revoke")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RevokeAccessAsync(
        [FromBody] UserGrantAccessReq req,
        CancellationToken ct)
    {
        var success = await _userAccessService.RevokeAccessFromUserAsync(req, UserId, ct);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Listează toate access-urile ale unui utilizator.
    /// </summary>
    [HttpGet("user/{userId:guid}")]
    [ProducesResponseType(typeof(List<AccessDto>), 200)]
    public async Task<ActionResult<List<AccessDto>>> GetAccessesForUserAsync(
        Guid userId,
        CancellationToken ct)
    {
        var list = await _userAccessService.GetAccessesForUserAsync(userId, ct);
        return Ok(list);
    }

    /// <summary>
    /// Listează access-urile paginat pentru un utilizator.
    /// </summary>
    [HttpGet("user/{userId:guid}/paged")]
    [ProducesResponseType(typeof(PagedResponse<AccessDto>), 200)]
    public async Task<ActionResult<PagedResponse<AccessDto>>> GetAccessesForUserPagedAsync(
        Guid userId,
        [FromQuery] PagedRequest req,
        CancellationToken ct)
    {
        var page = await _userAccessService.GetUserAccessesPagedAsync(userId, req, ct);
        return Ok(page);
    }
}