using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;

namespace MobID.MainGateway.Services.Interfaces;

public interface IOrganizationAccessShareService
{
    Task<OrganizationAccessShareDto> ShareAccessWithOrganizationAsync(AccessShareReq req, Guid grantedByUserId, CancellationToken ct = default);

    Task<bool> RevokeSharedAccessAsync(AccessShareReq req, Guid revokedByUserId, CancellationToken ct = default);
}
