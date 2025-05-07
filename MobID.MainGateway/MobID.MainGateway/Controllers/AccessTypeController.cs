using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccessTypeController : ControllerBase
    {
        private readonly IAccessTypeService _accessTypeService;
        public AccessTypeController(IAccessTypeService accessTypeService)
        {
            _accessTypeService = accessTypeService;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllTypes(CancellationToken ct)
        {
            try
            {
                var types = await _accessTypeService.GetAllTypes(ct);
                return Ok(types);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
