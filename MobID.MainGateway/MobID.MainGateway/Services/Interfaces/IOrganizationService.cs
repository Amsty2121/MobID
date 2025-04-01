using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;

namespace MobID.MainGateway.Services.Interfaces
{
    public interface IOrganizationService
    {
        Task<OrganizationDto> CreateOrganization(OrganizationCreateReq request, CancellationToken ct = default);
        Task<OrganizationDto?> GetOrganizationById(Guid organizationId, CancellationToken ct = default);
        Task<List<OrganizationDto>> GetAllOrganizations(CancellationToken ct = default);
        Task<bool> AddUserToOrganization(Guid organizationId, Guid userId, CancellationToken ct = default);
        Task<bool> RemoveUserFromOrganization(Guid organizationId, Guid userId, CancellationToken ct = default);
        Task<List<OrganizationUserDto>> GetUsersForOrganization(Guid organizationId, CancellationToken ct = default);
        Task<PagedResponse<OrganizationDto>> GetOrganizationsPaged(PagedRequest pagedRequest, CancellationToken ct = default);
    }
}
