using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos.Rsp;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    public AuthController(IAuthService authService) => _authService = authService;

    /// <summary>
    /// Authenticates a user with email/username and password, returns JWT + refresh token.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserLoginRsp), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> LoginAsync(
        [FromBody] UserLoginReq request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var rsp = await _authService.LoginAsync(request, ct);
            return Ok(rsp);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Issues a new JWT using a valid refresh token.
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserLoginRsp), 200)]
    [ProducesResponseType(400)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> RefreshTokenAsync(
        [FromQuery] string refreshToken,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return BadRequest(new { message = "Refresh token is required." });

        try
        {
            var rsp = await _authService.RefreshTokenAsync(refreshToken, ct);
            return Ok(rsp);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Revokes a refresh token (logout).
    /// </summary>
    [HttpPost("revoke")]
    [Authorize]
    [ProducesResponseType(204)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> RevokeTokenAsync(
        [FromQuery] string refreshToken,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(refreshToken))
            return BadRequest(new { message = "Refresh token is required." });

        await _authService.RevokeTokenAsync(refreshToken, ct);
        return NoContent();
    }

    /// <summary>
    /// Registers a new user and assigns the requested roles.
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(UserRegisterRsp), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> RegisterAsync(
        [FromBody] UserRegisterReq request,
        CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var rsp = await _authService.RegisterAsync(request, ct);
            return Created(string.Empty, rsp);
        }
        catch (InvalidOperationException ex)
        {
            // e.g. email already in use
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Verifică dacă JWT-ul din Authorization header este valid.
    /// </summary>
    [HttpGet("token-verify")]
    [Authorize]
    [ProducesResponseType(200)]
    [ProducesResponseType(401)]
    public IActionResult VerifyToken()
    {
        return Ok();
    }
}