using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos.Rsp;

namespace MobID.MainGateway.Services.Interfaces;

public interface IAuthService
{
    Task<UserLoginRsp> LoginAsync(UserLoginReq request, CancellationToken ct = default);
    Task<UserLoginRsp> RefreshTokenAsync(string refreshToken, CancellationToken ct = default);
    Task RevokeTokenAsync(string refreshToken, CancellationToken ct = default);
    Task<UserRegisterRsp> RegisterAsync(UserRegisterReq request, CancellationToken ct = default);
}