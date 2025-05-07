using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;

namespace MobID.MainGateway.Services.Interfaces
{
    public interface IAccessService
    {
        Task<AccessDto> CreateAccess(AccessCreateReq request, Guid creatorId, CancellationToken ct = default);
        Task<AccessDto?> GetAccessById(Guid accessId, CancellationToken ct = default);
        Task<List<AccessDto>> GetAccessesForOrganization(Guid organizationId, CancellationToken ct = default);
        Task<bool> DeactivateAccess(Guid accessId, CancellationToken ct = default);
        Task<PagedResponse<AccessDto>> GetAccessesPaged(PagedRequest pagedRequest, CancellationToken ct = default);
    }
}
