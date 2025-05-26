using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;

namespace MobID.MainGateway.Services.Interfaces;

public interface IAccessService
{
    Task<AccessDto> CreateAccessAsync(AccessCreateReq request, Guid createdByUserId, CancellationToken ct = default);
    Task<AccessDto?> GetAccessByIdAsync(Guid accessId, CancellationToken ct = default);
    Task<List<AccessDto>> GetAccessesForOrganizationAsync(Guid organizationId, CancellationToken ct = default);
    Task<PagedResponse<AccessDto>> GetAccessesPagedAsync(PagedRequest request, CancellationToken ct = default);
    Task<bool> DeactivateAccessAsync(Guid accessId, CancellationToken ct = default);
}
