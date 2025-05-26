// IAccessShareService.cs
using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;

namespace MobID.MainGateway.Services.Interfaces;

public interface IAccessShareService
{
    Task<OrganizationAccessShareDto> ShareAccessWithOrganizationAsync(
        AccessShareReq req,
        Guid grantedByUserId,
        CancellationToken ct = default);

    Task<bool> RevokeSharedAccessAsync(
        AccessShareReq req,
        Guid revokedByUserId,
        CancellationToken ct = default);

    Task<List<OrganizationAccessShareDto>> GetSharesForAccessAsync(
        Guid accessId,
        CancellationToken ct = default);

    Task<List<OrganizationAccessShareDto>> GetSharesForOrganizationAsync(
        Guid organizationId,
        CancellationToken ct = default);
}
