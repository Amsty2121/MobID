using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginReq request, CancellationToken ct)
        {
            try
            {
                var result = await _authService.Login(request, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Log exception if needed
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromQuery] string refreshToken, CancellationToken ct)
        {
            try
            {
                var result = await _authService.RefreshToken(refreshToken, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("revoke")]
        public async Task<IActionResult> RevokeToken([FromQuery] string refreshToken, CancellationToken ct)
        {
            try
            {
                await _authService.RevokeToken(refreshToken, ct);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegisterReq request, CancellationToken ct)
        {
            try
            {
                var result = await _authService.Register(request, ct);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("tokenVerify")]
        public async Task<IActionResult> TokenVerify( CancellationToken ct)
        {
            return Ok();
        }
    }
}
