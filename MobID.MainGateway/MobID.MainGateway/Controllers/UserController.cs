using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MobID.MainGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddUser([FromBody] Models.Dtos.Req.UserAddReq request, CancellationToken ct)
        {
            try
            {
                var user = await _userService.AddUser(request, ct);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken ct)
        {
            try
            {
                bool success = await _userService.DeleteUser(userId, ct);
                return success ? NoContent() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(Guid userId, CancellationToken ct)
        {
            try
            {
                var user = await _userService.GetUserById(userId, ct);
                return user == null ? NotFound() : Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("paged")]
        public async Task<IActionResult> GetUsersPaged([FromQuery] Models.Dtos.PagedRequest request, CancellationToken ct)
        {
            try
            {
                var pagedUsers = await _userService.GetUsersPaged(request, ct);
                return Ok(pagedUsers);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("assignRole")]
        public async Task<IActionResult> AssignRoleToUser([FromQuery] Guid userId, [FromQuery] Guid roleId, CancellationToken ct)
        {
            try
            {
                bool success = await _userService.AssignRoleToUser(userId, roleId, ct);
                return success ? Ok() : BadRequest("Atribuire nereușită sau rol deja atribuit.");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("removeRole")]
        public async Task<IActionResult> RemoveRoleFromUser([FromQuery] Guid userId, [FromQuery] Guid roleId, CancellationToken ct)
        {
            try
            {
                bool success = await _userService.RemoveRoleFromUser(userId, roleId, ct);
                return success ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{userId}/roles")]
        public async Task<IActionResult> GetUserRoles(Guid userId, CancellationToken ct)
        {
            try
            {
                var roles = await _userService.GetUserRoles(userId, ct);
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
