using MobID.MainGateway.Models.Dtos;

public interface IOrganizationAccessShareService
{
    Task<bool> ShareAccessWithOrganizationAsync(AccessShareReq req, Guid userId, CancellationToken ct = default);
    Task<bool> RevokeSharedAccessAsync(AccessShareReq req, CancellationToken ct = default);
    Task<List<OrganizationAccessShareDto>> GetAccessesSharedToOrganizationAsync(Guid organizationId, CancellationToken ct = default);
    Task<List<OrganizationAccessShareDto>> GetSharedAccessesBetweenOrganizationsAsync(Guid sourceOrgId, Guid targetOrgId, CancellationToken ct = default);
}
