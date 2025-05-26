// src/Controllers/OrganizationController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _orgService;
        public OrganizationController(IOrganizationService orgService)
            => _orgService = orgService;

        /// <summary>
        /// Creează o organizaţie nouă.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(OrganizationDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<OrganizationDto>> CreateOrganizationAsync(
            [FromBody] OrganizationCreateReq req,
            CancellationToken ct)
        {
            try
            {
                var dto = await _orgService.CreateOrganizationAsync(req, ct);
                // 201 Created cu locația noii resurse  
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Returnează o organizaţie după ID.
        /// </summary>
        [HttpGet("{organizationId:guid}")]
        [ProducesResponseType(typeof(OrganizationDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<OrganizationDto>> GetOrganizationByIdAsync(
            Guid organizationId,
            CancellationToken ct)
        {
            try
            {
                var dto = await _orgService.GetOrganizationByIdAsync(organizationId, ct);
                return dto == null ? NotFound() : Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Listează organizaţiile paginat.
        /// </summary>
        [HttpGet("paged")]
        [ProducesResponseType(typeof(PagedResponse<OrganizationDto>), 200)]
        public async Task<ActionResult<PagedResponse<OrganizationDto>>> GetOrganizationsPagedAsync(
            [FromQuery] PagedRequest req,
            CancellationToken ct)
        {
            try
            {
                var page = await _orgService.GetOrganizationsPagedAsync(req, ct);
                return Ok(page);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Listează toate organizaţiile.
        /// </summary>
        [HttpGet("all")]
        [ProducesResponseType(typeof(List<OrganizationDto>), 200)]
        public async Task<ActionResult<List<OrganizationDto>>> GetAllOrganizationsAsync(
            CancellationToken ct)
        {
            try
            {
                var list = await _orgService.GetAllOrganizationsAsync(ct);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Actualizează datele unei organizaţii.
        /// </summary>
        [HttpPatch]
        [ProducesResponseType(typeof(OrganizationDto), 200)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<OrganizationDto>> UpdateOrganizationAsync(
            [FromBody] OrganizationUpdateReq req,
            CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var dto = await _orgService.UpdateOrganizationAsync(req, ct);
                return Ok(dto);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deactivează (soft-delete) o organizaţie.
        /// </summary>
        [HttpDelete("{organizationId:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeactivateOrganizationAsync(
            Guid organizationId,
            CancellationToken ct)
        {
            try
            {
                var success = await _orgService.DeactivateOrganizationAsync(organizationId, ct);
                return success ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ─── Gestionare membri ────────────────────────────────────────────────────

        /// <summary>
        /// Adaugă un utilizator într-o organizaţie.
        /// </summary>
        [HttpPost("{organizationId:guid}/users")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> AddUserToOrganizationAsync(
            Guid organizationId,
            [FromBody] OrganizationAddUserReq req,
            CancellationToken ct)
        {
            try
            {
                var success = await _orgService.AddUserToOrganizationAsync(organizationId, req, ct);
                if (!success)
                    return BadRequest(new { message = "Utilizator deja membru sau invalid." });
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Elimină un utilizator dintr-o organizaţie (soft-delete).
        /// </summary>
        [HttpDelete("{organizationId:guid}/users/{userId:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RemoveUserFromOrganizationAsync(
            Guid organizationId,
            Guid userId,
            CancellationToken ct)
        {
            try
            {
                var success = await _orgService.RemoveUserFromOrganizationAsync(organizationId, userId, ct);
                return success ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Listează toţi membrii unei organizaţii.
        /// </summary>
        [HttpGet("{organizationId:guid}/users")]
        [ProducesResponseType(typeof(List<OrganizationUserDto>), 200)]
        public async Task<ActionResult<List<OrganizationUserDto>>> GetUsersForOrganizationAsync(
            Guid organizationId,
            CancellationToken ct)
        {
            try
            {
                var list = await _orgService.GetUsersForOrganizationAsync(organizationId, ct);
                return Ok(list);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ─── Accese ────────────────────────────────────────────────────────────────

        /// <summary>
        /// Listează accesele proprii ale organizației.
        /// </summary>
        [HttpGet("{organizationId:guid}/accesses")]
        [ProducesResponseType(typeof(List<AccessDto>), 200)]
        public async Task<ActionResult<List<AccessDto>>> GetOrganizationAccessesAsync(
            Guid organizationId,
            CancellationToken ct)
        {
            var list = await _orgService.GetOrganizationAccessesAsync(organizationId, ct);
            return Ok(list);
        }

        /// <summary>
        /// Listează accesele partajate către organizație.
        /// </summary>
        [HttpGet("{organizationId:guid}/accesses/shared")]
        [ProducesResponseType(typeof(List<OrganizationAccessShareDto>), 200)]
        public async Task<ActionResult<List<OrganizationAccessShareDto>>> GetAccessesSharedToOrganizationAsync(
            Guid organizationId,
            CancellationToken ct)
        {
            var shares = await _orgService.GetAccessesSharedToOrganizationAsync(organizationId, ct);
            return Ok(shares);
        }

        /// <summary>
        /// Listează toate accesele (proprii + partajate) ale organizației.
        /// </summary>
        [HttpGet("{organizationId:guid}/accesses/all")]
        [ProducesResponseType(typeof(List<AccessDto>), 200)]
        public async Task<ActionResult<List<AccessDto>>> GetAllOrganizationAccessesAsync(
            Guid organizationId,
            CancellationToken ct)
        {
            var combined = await _orgService.GetAllOrganizationAccessesAsync(organizationId, ct);
            return Ok(combined);
        }
    }
}
