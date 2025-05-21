using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MobID.MainGateway.Configuration;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos.Rsp;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services;

public class AuthService : IAuthService
{
    private readonly IGenericRepository<User> _userRepo;
    private readonly IGenericRepository<Role> _roleRepo;
    private readonly IGenericRepository<UserRole> _userRoleRepo;
    private readonly IGenericRepository<RefreshToken> _tokenRepo;
    private readonly AuthOptions _options;

    public AuthService(
        IGenericRepository<User> userRepo,
        IGenericRepository<Role> roleRepo,
        IGenericRepository<UserRole> userRoleRepo,
        IOptions<AuthOptions> options,
        IGenericRepository<RefreshToken> tokenRepo)
    {
        _userRepo = userRepo;
        _roleRepo = roleRepo;
        _userRoleRepo = userRoleRepo;
        _tokenRepo = tokenRepo;
        _options = options.Value;
    }

    /// <inheritdoc/>
    public async Task<UserLoginRsp> LoginAsync(UserLoginReq request, CancellationToken ct = default)
    {
        var user = await _userRepo.FirstOrDefault(u => u.Email == request.Login, ct)
                   ?? throw new UnauthorizedAccessException("Invalid credentials.");
        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        var roles = await _userRoleRepo.GetWhereWithInclude(
            ur => ur.UserId == user.Id && ur.IsActive, ct, ur => ur.Role);

        var jwt = GenerateJwt(user, roles.Select(r => r.Role.Name).ToList());
        var refresh = await CreateOrUpdateRefreshTokenAsync(user.Id, ct);

        return new UserLoginRsp(user, jwt, refresh.Token);
    }

    /// <inheritdoc/>
    public async Task<UserLoginRsp> RefreshTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var token = await _tokenRepo.FirstOrDefault(t => t.Token == refreshToken, ct)
                    ?? throw new UnauthorizedAccessException("Invalid refresh token.");
        if (token.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expired.");

        var user = await _userRepo.GetById(token.UserId, ct)
                   ?? throw new UnauthorizedAccessException("User not found.");

        var roles = await _userRoleRepo.GetWhereWithInclude(
            ur => ur.UserId == user.Id && ur.IsActive, ct, ur => ur.Role);

        var jwt = GenerateJwt(user, roles.Select(r => r.Role.Name).ToList());

        // rotate refresh token
        token.Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        token.ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenLifetimeDays);
        await _tokenRepo.Update(token, ct);

        return new UserLoginRsp(user, jwt, token.Token);
    }

    /// <inheritdoc/>
    public async Task RevokeTokenAsync(string refreshToken, CancellationToken ct = default)
    {
        var token = await _tokenRepo.FirstOrDefault(t => t.Token == refreshToken, ct);
        if (token != null)
            await _tokenRepo.Remove(token, ct);
    }

    /// <inheritdoc/>
    public async Task<UserRegisterRsp> RegisterAsync(UserRegisterReq request, CancellationToken ct = default)
    {
        if (await _userRepo.FirstOrDefault(u => u.Email == request.Email, ct) != null)
            throw new InvalidOperationException("Email already in use.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };
        await _userRepo.Add(user, ct);

        var roles = await _roleRepo.GetWhere(r => request.Roles.Contains(r.Name), ct);
        foreach (var r in roles)
        {
            await _userRoleRepo.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = r.Id,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }, ct);
        }

        return new UserRegisterRsp(user, roles.Select(r => r.Name).ToList());
    }

    // ─── Helpers ─────────────────────────────────────────────────────

    private string GenerateJwt(User user, IList<string> roles)
    {
        var claims = new List<Claim>
            {
                new Claim(nameof(User.Id),       user.Id.ToString()),
                new Claim(nameof(User.Username), user.Username)
            };
        claims.AddRange(roles.Select(r => new Claim("Roles", r)));

        var creds = new SigningCredentials(
            _options.GetSymmetricSecurityKey(),
            SecurityAlgorithms.HmacSha256);

        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(_options.TokenLifetimeDays),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }

    private async Task<RefreshToken> CreateOrUpdateRefreshTokenAsync(Guid userId, CancellationToken ct)
    {
        var existing = await _tokenRepo.FirstOrDefault(t => t.UserId == userId, ct);
        if (existing == null)
        {
            var rt = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenLifetimeDays),
                CreatedAt = DateTime.UtcNow
            };
            await _tokenRepo.Add(rt, ct);
            return rt;
        }

        existing.Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        existing.ExpiresAt = DateTime.UtcNow.AddDays(_options.RefreshTokenLifetimeDays);
        await _tokenRepo.Update(existing, ct);
        return existing;
    }
}