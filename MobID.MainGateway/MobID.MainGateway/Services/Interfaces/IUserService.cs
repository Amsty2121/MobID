using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos.Rsp;

namespace MobID.MainGateway.Services.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> AddUser(UserAddReq userDto, CancellationToken ct = default);
        Task<bool> DeleteUser(Guid userId, CancellationToken ct = default);
        Task<UserDto?> GetUserById(Guid userId, CancellationToken ct = default);
        Task<UsersPagedRsp> GetUsersPaged(int limit, int offset, CancellationToken ct = default);
    }
}
