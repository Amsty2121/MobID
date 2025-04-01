using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace MobID.MainGateway.Services.Interfaces
{
    public interface IAccessService
    {
        Task<AccessDto> CreateAccess(AccessCreateReq request, CancellationToken ct = default);
        Task<AccessDto?> GetAccessById(Guid accessId, CancellationToken ct = default);
        Task<List<AccessDto>> GetAccessesForOrganization(Guid organizationId, CancellationToken ct = default);
        Task<bool> DeactivateAccess(Guid accessId, CancellationToken ct = default);
        Task<PagedResponse<AccessDto>> GetAccessesPaged(PagedRequest pagedRequest, CancellationToken ct = default);
    }
}
