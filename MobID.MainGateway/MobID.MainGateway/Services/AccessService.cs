using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services
{
    public class AccessService : IAccessService
    {
        private readonly IGenericRepository<Access> _accessRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<AccessType> _accessTypeRepository;
        private readonly IGenericRepository<Organization> _organizationRepository;
        private readonly IGenericRepository<QrCode> _qrCodeRepository;

        public AccessService(
            IGenericRepository<Access> accessRepository,
            IGenericRepository<User> userRepository,
            IGenericRepository<AccessType> accessTypeRepository,
            IGenericRepository<Organization> organizationRepository,
            IGenericRepository<QrCode> qrCodeRepository)
        {
            _accessRepository = accessRepository;
            _userRepository = userRepository;
            _accessTypeRepository = accessTypeRepository;
            _organizationRepository = organizationRepository;
            _qrCodeRepository = qrCodeRepository;
        }

        public async Task<AccessDto> CreateAccess(AccessCreateReq request, Guid creatorId, CancellationToken ct = default)
        {
            var accessType = await _accessTypeRepository.GetById(request.AccessTypeId, ct)
                ?? throw new InvalidOperationException("Access type not found.");
            
            var user = await _userRepository.GetById(creatorId, ct)
                ?? throw new InvalidOperationException("User Creator not found.");

            var organization = await _organizationRepository.GetById(request.OrganizationId, ct)
                ?? throw new InvalidOperationException("Organization not found.");

            var access = new Access
            {
                Id = Guid.NewGuid(),
                CreatedBy = user.Id,
                OrganizationId = organization.Id,
                AccessTypeId = accessType.Id,
                ExpirationDateTime = request.ExpirationDate,
                CreatedAt = DateTime.UtcNow
            };

            await _accessRepository.Add(access, ct);
            return new AccessDto(access);
        }

        public async Task<AccessDto?> GetAccessById(Guid accessId, CancellationToken ct = default)
        {
            var access = await _accessRepository.GetByIdWithInclude(
                accessId, ct, a => a.Organization, a => a.Creator, a => a.AccessType, a => a.QrCodes);
            return access == null ? null : new AccessDto(access);
        }

        public async Task<List<AccessDto>> GetAccessesForOrganization(Guid organizationId, CancellationToken ct = default)
        {
            var accesses = await _accessRepository.GetWhereWithInclude(
                a => a.OrganizationId == organizationId && a.DeletedAt == null, ct,
                a => a.Organization, a => a.Creator, a => a.AccessType, a => a.QrCodes);
            return accesses.Select(a => new AccessDto(a)).ToList();
        }

        public async Task<bool> DeactivateAccess(Guid accessId, CancellationToken ct = default)
        {
            var access = await _accessRepository.GetById(accessId, ct);
            if (access == null) return false;
            access.DeletedAt = DateTime.UtcNow;
            await _accessRepository.Update(access, ct);
            return true;
        }

        public async Task<PagedResponse<AccessDto>> GetAccessesPaged(PagedRequest pagedRequest, CancellationToken ct = default)
        {
            int offset = pagedRequest.PageIndex * pagedRequest.PageSize;
            var queryList = (await _accessRepository.GetWhereWithInclude(a => a.DeletedAt == null, ct, x => x.AccessType, y => y.Creator))?.ToList() ?? new List<Access>();
            int total = queryList.Count;
            var accesses = queryList
                                .Skip(offset)
                                .Take(pagedRequest.PageSize)
                                .Select(a => new AccessDto(a))
                                .ToList();
            return new PagedResponse<AccessDto>(pagedRequest.PageIndex, pagedRequest.PageSize, total, accesses);
        }
    }
}
