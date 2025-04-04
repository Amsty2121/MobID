using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MobID.MainGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrganizationController : ControllerBase
    {
        private readonly IOrganizationService _orgService;
        public OrganizationController(IOrganizationService orgService) => _orgService = orgService;

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrganization([FromBody] OrganizationCreateReq request, CancellationToken ct)
        {
            try
            {
                var org = await _orgService.CreateOrganization(request, ct);
                return Ok(org);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateOrganization([FromBody] OrganizationUpdateReq request, CancellationToken ct)
        {
            try
            {
                var updatedOrg = await _orgService.UpdateOrganization(request, ct);
                return Ok(updatedOrg);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{organizationId}")]
        public async Task<IActionResult> DeleteOrganization(Guid organizationId, CancellationToken ct)
        {
            try
            {
                bool success = await _orgService.DeleteOrganization(organizationId, ct);
                return success ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{organizationId}")]
        public async Task<IActionResult> GetOrganizationById(Guid organizationId, CancellationToken ct)
        {
            try
            {
                var org = await _orgService.GetOrganizationById(organizationId, ct);
                return org == null ? NotFound() : Ok(org);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllOrganizations(CancellationToken ct)
        {
            try
            {
                var orgs = await _orgService.GetAllOrganizations(ct);
                return Ok(orgs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetOrganizationsPaged([FromQuery] PagedRequest request, CancellationToken ct)
        {
            try
            {
                var pagedOrgs = await _orgService.GetOrganizationsPaged(request, ct);
                return Ok(pagedOrgs);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{organizationId}/addUser")]
        public async Task<IActionResult> AddUserToOrganization(Guid organizationId, [FromQuery] Guid userId, CancellationToken ct)
        {
            try
            {
                bool success = await _orgService.AddUserToOrganization(organizationId, userId, ct);
                return success ? Ok() : BadRequest("Could not add user to organization.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{organizationId}/removeUser")]
        public async Task<IActionResult> RemoveUserFromOrganization(Guid organizationId, [FromQuery] Guid userId, CancellationToken ct)
        {
            try
            {
                bool success = await _orgService.RemoveUserFromOrganization(organizationId, userId, ct);
                return success ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{organizationId}/users")]
        public async Task<IActionResult> GetUsersForOrganization(Guid organizationId, CancellationToken ct)
        {
            try
            {
                var users = await _orgService.GetUsersForOrganization(organizationId, ct);
                return Ok(users);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
