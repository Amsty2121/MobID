using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MobID.MainGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
        => _userService = userService;

    /// <summary>
    /// Crează un utilizator nou.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(UserDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<UserDto>> CreateUserAsync(
        [FromBody] UserAddReq req,
        CancellationToken ct)
    {
        var dto = await _userService.CreateUserAsync(req, ct);
        return Ok(dto);
    }

    /// <summary>
    /// Obține detaliile unui utilizator după ID.
    /// </summary>
    [HttpGet("{userId:guid}")]
    [ProducesResponseType(typeof(UserDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<UserDto>> GetUserByIdAsync(
        Guid userId,
        CancellationToken ct)
    {
        var dto = await _userService.GetUserByIdAsync(userId, ct);
        return dto == null
            ? NotFound()
            : Ok(dto);
    }

    /// <summary>
    /// Listează utilizatorii paginat.
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<UserDto>), 200)]
    public async Task<ActionResult<PagedResponse<UserDto>>> GetUsersPagedAsync(
        [FromQuery] PagedRequest req,
        CancellationToken ct)
    {
        var page = await _userService.GetUsersPagedAsync(req, ct);
        return Ok(page);
    }

    /// <summary>
    /// Dezactivează (soft-delete) un utilizator.
    /// </summary>
    [HttpDelete("{userId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeactivateUserAsync(
        Guid userId,
        CancellationToken ct)
    {
        var success = await _userService.DeactivateUserAsync(userId, ct);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Atribuie un rol unui utilizator.
    /// </summary>
    [HttpPost("{userId:guid}/roles/{roleId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AssignRoleAsync(
        Guid userId,
        Guid roleId,
        CancellationToken ct)
    {
        var success = await _userService.AssignRoleToUserAsync(userId, roleId, ct);
        if (!success)
            return BadRequest(new { message = "Nu s-a putut atribui rolul (poate există deja sau date invalide)." });
        return NoContent();
    }

    /// <summary>
    /// Înlătură un rol de la un utilizator.
    /// </summary>
    [HttpDelete("{userId:guid}/roles/{roleId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> RemoveRoleAsync(
        Guid userId,
        Guid roleId,
        CancellationToken ct)
    {
        var success = await _userService.RemoveRoleFromUserAsync(userId, roleId, ct);
        return success ? NoContent() : NotFound();
    }

    /// <summary>
    /// Listează rolurile unui utilizator.
    /// </summary>
    [HttpGet("{userId:guid}/roles")]
    [ProducesResponseType(typeof(List<string>), 200)]
    public async Task<ActionResult<List<string>>> GetUserRolesAsync(
        Guid userId,
        CancellationToken ct)
    {
        var roles = await _userService.GetUserRolesAsync(userId, ct);
        return Ok(roles);
    }

    [HttpGet("{userId}/all-accesses")]
    [ProducesResponseType(typeof(List<AccessDto>), 200)]
    public async Task<ActionResult<List<AccessDto>>> GetAllUserAccesses(Guid userId, CancellationToken ct)
    {
        var list = await _userService.GetAllUserAccessesAsync(userId, ct);
        return Ok(list);
    }

    [HttpGet("{userId:guid}/organizations")]
    [ProducesResponseType(typeof(List<OrganizationDto>), 200)]
    public async Task<ActionResult<List<OrganizationDto>>> GetUserOrganizationsAsync(
        Guid userId,
        CancellationToken ct)
    {
        var list = await _userService.GetUserOrganizationsAsync(userId, ct);
        return Ok(list);
    }
}
