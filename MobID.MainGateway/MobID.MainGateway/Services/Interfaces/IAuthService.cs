using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos.Rsp;

namespace MobID.MainGateway.Services.Interfaces
{
    public interface IAuthService
    {
        Task<UserLoginRsp> Login(UserLoginReq request, CancellationToken ct = default);
        Task<UserLoginRsp> RefreshToken(string refreshToken, CancellationToken ct = default);
        Task RevokeToken(string refreshToken, CancellationToken ct = default);
        Task<UserRegisterRsp> Register(UserRegisterReq request, CancellationToken ct = default);
    }
}
