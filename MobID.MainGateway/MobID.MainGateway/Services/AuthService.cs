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

namespace MobID.MainGateway.Services
{
    public class AuthService : IAuthService
    {
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<UserRole> _userRoleRepository;
        private readonly IGenericRepository<RefreshToken> _tokenRepository;
        private readonly AuthOptions _authOptions;

        public AuthService(
            IGenericRepository<User> userRepository,
            IGenericRepository<Role> roleRepository,
            IGenericRepository<UserRole> userRoleRepository,
            IOptions<AuthOptions> authOptions,
            IGenericRepository<RefreshToken> tokenRepository)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _userRoleRepository = userRoleRepository;
            _authOptions = authOptions.Value;
            _tokenRepository = tokenRepository;
        }

        public async Task<UserLoginRsp> Login(UserLoginReq request, CancellationToken ct = default)
        {
            var user = await _userRepository.FirstOrDefault(u => u.Email == request.Login, ct);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Invalid credentials.");
            }

            var roles = await _userRoleRepository.GetWhereWithInclude(r => r.UserId == user.Id, ct, r => r.Role);
            var jwtToken = GenerateJwtToken(user, roles.Select(r => r.Role.Name).ToList());
            var refreshToken = await ProcessRefreshToken(user.Id, ct);

            return new UserLoginRsp(user, jwtToken, refreshToken.Token);
        }

        private async Task<RefreshToken> ProcessRefreshToken(Guid userId, CancellationToken ct)
        {
            var refreshToken = await _tokenRepository.FirstOrDefault(t => t.UserId == userId, ct);

            if (refreshToken == null)
            {
                refreshToken = GenerateRefreshToken(userId);
                await _tokenRepository.Add(refreshToken, ct);
            }
            else
            {
                refreshToken.Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
                refreshToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
                await _tokenRepository.Update(refreshToken, ct);
            }

            return refreshToken;
        }


        public async Task<UserLoginRsp> RefreshToken(string refreshToken, CancellationToken ct = default)
        {
            var token = await _tokenRepository.FirstOrDefault(r => r.Token == refreshToken, ct);
            if (token == null || token.ExpiresAt < DateTime.UtcNow)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }

            var user = await _userRepository.GetById(token.UserId, ct)
                ?? throw new UnauthorizedAccessException("User not found.");

            var roles = await _userRoleRepository.GetWhereWithInclude(r => r.UserId == user.Id, ct, r => r.Role);
            var jwtToken = GenerateJwtToken(user, roles.Select(r => r.Role.Name).ToList());

            token.Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            token.ExpiresAt = DateTime.UtcNow.AddDays(7);
            await _tokenRepository.Update(token, ct);

            return new UserLoginRsp(user, jwtToken, token.Token);
        }

        public async Task RevokeToken(string refreshToken, CancellationToken ct = default)
        {
            var token = await _tokenRepository.FirstOrDefault(r => r.Token == refreshToken, ct);
            if (token != null)
            {
                await _tokenRepository.Remove(token, ct);
            }
        }

        private string GenerateJwtToken(User user, ICollection<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(nameof(User.Id), user.Id.ToString()),
                new Claim(nameof(User.Username), user.Username),
            };

            claims.AddRange(roles.Select(role => new Claim("Roles", role)));

            var tokenHandler = new JwtSecurityTokenHandler();
            var signinCredentials = new SigningCredentials(_authOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256);
            var jwtSecurityToken = new JwtSecurityToken(
                _authOptions.Issuer,
                _authOptions.Audience,
                claims,
                expires: DateTime.UtcNow.AddDays(_authOptions.TokenLifetime),
                signingCredentials: signinCredentials
            );

            return tokenHandler.WriteToken(jwtSecurityToken);
        }

        private RefreshToken GenerateRefreshToken(Guid userId)
        {
            return new RefreshToken
            {
                Id = Guid.NewGuid(),
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };
        }

        public async Task<UserRegisterRsp> Register(UserRegisterReq request, CancellationToken ct = default)
        {
            if (await _userRepository.FirstOrDefault(u => u.Email == request.Email, ct) != null)
            {
                throw new InvalidOperationException("User with the same email already exists.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            };

            await _userRepository.Add(user, ct);

            var roles = await _roleRepository.GetWhere(r => request.Roles.Contains(r.Name), ct);
            foreach (var role in roles)
            {
                await _userRoleRepository.Add(new UserRole { UserId = user.Id, RoleId = role.Id, IsActive = true }, ct);
            }

            return new UserRegisterRsp(user, roles.Select(r => r.Name).ToList());
        }
    }
}
