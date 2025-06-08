using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;

namespace MobID.MainGateway.Services.Interfaces;

public interface IAccessService
{
    Task<AccessDto> CreateAccessAsync(AccessCreateReq req, Guid userId, CancellationToken ct = default);
    Task<AccessDto?> GetByIdAsync(Guid accessId, CancellationToken ct = default);
    Task<List<AccessDto>> GetAccessesForOrganizationAsync(Guid organizationId, CancellationToken ct = default);
    Task<int> DeactivateAccessAsync(Guid accessId, Guid userId, CancellationToken ct = default);
    Task<AccessDto?> UpdateAccessAsync(AccessUpdateReq req, Guid userId, CancellationToken ct = default);
}
