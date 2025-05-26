using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;

namespace MobID.MainGateway.Services.Interfaces;

public interface IOrganizationService
{
    Task<OrganizationDto> CreateOrganizationAsync(OrganizationCreateReq request, CancellationToken ct = default);
    Task<OrganizationDto?> GetOrganizationByIdAsync(Guid organizationId, CancellationToken ct = default);
    Task<PagedResponse<OrganizationDto>> GetOrganizationsPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<List<OrganizationDto>> GetAllOrganizationsAsync(CancellationToken ct = default);

    Task<OrganizationDto> UpdateOrganizationAsync(OrganizationUpdateReq request, CancellationToken ct = default);
    Task<bool> DeactivateOrganizationAsync(Guid organizationId, CancellationToken ct = default);


    Task<bool> AddUserToOrganizationAsync(Guid organizationId, OrganizationAddUserReq request, CancellationToken ct = default);
    Task<bool> RemoveUserFromOrganizationAsync(Guid organizationId, Guid userId, CancellationToken ct = default);

    Task<List<OrganizationUserDto>> GetUsersForOrganizationAsync(Guid organizationId, CancellationToken ct = default);

    Task<List<AccessDto>> GetOrganizationAccessesAsync(Guid organizationId, CancellationToken ct = default);
    Task<List<OrganizationAccessShareDto>> GetAccessesSharedToOrganizationAsync(Guid organizationId, CancellationToken ct = default);
    Task<List<AccessDto>> GetAllOrganizationAccessesAsync(Guid organizationId, CancellationToken ct = default);



}
