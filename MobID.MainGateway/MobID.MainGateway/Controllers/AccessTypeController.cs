using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AccessTypeController : ControllerBase
{
    private readonly IAccessTypeService _accessTypeService;

    public AccessTypeController(IAccessTypeService accessTypeService)
        => _accessTypeService = accessTypeService;

    /// <summary>
    /// Returnează toate tipurile de acces disponibile.
    /// </summary>
    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<AccessTypeDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<IEnumerable<AccessTypeDto>>> GetAllAsync(CancellationToken ct)
    {
        try
        {
            var types = await _accessTypeService.GetAllTypesAsync(ct);
            return Ok(types);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}