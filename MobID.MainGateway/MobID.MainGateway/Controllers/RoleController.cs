using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos.Rsp;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Services.Interfaces;
using MobID.MainGateway.Models.Dtos.Req;

namespace MobID.MainGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
        => _roleService = roleService;

    /// <summary>
    /// Creează un rol nou.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RoleDto), 201)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<RoleDto>> CreateRoleAsync(
        [FromBody] RoleCreateReq req,
        CancellationToken ct)
    {
        try
        {
            var dto = await _roleService.CreateRoleAsync(req, ct);
            return Ok(dto);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Obține un rol după ID.
    /// </summary>
    [HttpGet("{roleId:guid}")]
    [ProducesResponseType(typeof(RoleDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<RoleDto>> GetRoleByIdAsync(
        Guid roleId,
        CancellationToken ct)
    {
        var dto = await _roleService.GetRoleByIdAsync(roleId, ct);
        return dto == null ? NotFound() : Ok(dto);
    }

    /// <summary>
    /// Obține un rol după nume.
    /// </summary>
    [HttpGet("by-name/{roleName}")]
    [ProducesResponseType(typeof(RoleDto), 200)]
    [ProducesResponseType(404)]
    public async Task<ActionResult<RoleDto>> GetRoleByNameAsync(
        string roleName,
        CancellationToken ct)
    {
        var dto = await _roleService.GetRoleByNameAsync(roleName, ct);
        return dto == null ? NotFound() : Ok(dto);
    }

    /// <summary>
    /// Listează toate rolurile.
    /// </summary>
    [HttpGet("all")]
    [ProducesResponseType(typeof(List<RoleDto>), 200)]
    public async Task<ActionResult<List<RoleDto>>> GetAllRolesAsync(
        CancellationToken ct)
    {
        var list = await _roleService.GetAllRolesAsync(ct);
        return Ok(list);
    }

    /// <summary>
    /// Listează rolurile paginat.
    /// </summary>
    [HttpGet("paged")]
    [ProducesResponseType(typeof(PagedResponse<RoleDto>), 200)]
    public async Task<ActionResult<PagedResponse<RoleDto>>> GetRolesPagedAsync(
        [FromQuery] PagedRequest req,
        CancellationToken ct)
    {
        var page = await _roleService.GetRolesPagedAsync(req, ct);
        return Ok(page);
    }

    /// <summary>
    /// Dezactivează (soft-delete) un rol.
    /// </summary>
    [HttpDelete("{roleId:guid}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> DeactivateRoleAsync(
        Guid roleId,
        CancellationToken ct)
    {
        var ok = await _roleService.DeactivateRoleAsync(roleId, ct);
        return ok ? NoContent() : NotFound();
    }
}