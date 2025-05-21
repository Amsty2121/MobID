using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Enums;

namespace MobID.MainGateway.Services.Interfaces;

public interface IUserAccessService
{
    Task<bool> GrantAccessToUserAsync(
            UserGrantAccessReq req,
            Guid grantedByUserId,
            CancellationToken ct = default,
            AccessGrantType grantType = AccessGrantType.DirectGrant);

    Task<bool> RevokeAccessFromUserAsync(
            UserGrantAccessReq req,
            Guid revokedByUserId,
            CancellationToken ct = default);

    Task<List<AccessDto>> GetAccessesForUserAsync(
        Guid userId,
        CancellationToken ct = default);

    Task<PagedResponse<AccessDto>> GetUserAccessesPagedAsync(
        Guid userId,
        PagedRequest request,
        CancellationToken ct = default);
}
