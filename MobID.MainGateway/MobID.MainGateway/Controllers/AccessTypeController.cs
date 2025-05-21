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
    [HttpGet]
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

    /// <summary>
    /// Returnează un tip de acces după ID.
    /// </summary>
    [HttpGet("{typeId:guid}")]
    [ProducesResponseType(typeof(AccessTypeDto), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<ActionResult<AccessTypeDto>> GetByIdAsync(Guid typeId, CancellationToken ct)
    {
        try
        {
            var dto = await _accessTypeService.GetTypeByIdAsync(typeId, ct);
            return dto is not null
                ? Ok(dto)
                : NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}