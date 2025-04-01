using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IGenericRepository<Organization> _organizationRepository;
        private readonly IGenericRepository<OrganizationUser> _organizationUserRepository;
        private readonly IGenericRepository<User> _userRepository;

        public OrganizationService(
            IGenericRepository<Organization> organizationRepository,
            IGenericRepository<OrganizationUser> organizationUserRepository,
            IGenericRepository<User> userRepository)
        {
            _organizationRepository = organizationRepository;
            _organizationUserRepository = organizationUserRepository;
            _userRepository = userRepository;
        }

        public async Task<OrganizationDto> CreateOrganization(OrganizationCreateReq request, CancellationToken ct = default)
        {
            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                OwnerId = request.OwnerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _organizationRepository.Add(organization, ct);
            return new OrganizationDto(organization);
        }

        public async Task<OrganizationDto?> GetOrganizationById(Guid organizationId, CancellationToken ct = default)
        {
            var organization = await _organizationRepository.GetByIdWithInclude(organizationId, ct, o => o.OrganizationUsers);
            return organization == null ? null : new OrganizationDto(organization);
        }

        public async Task<List<OrganizationDto>> GetAllOrganizations(CancellationToken ct = default)
        {
            var organizations = await _organizationRepository.GetWhere(o => o.DeletedAt == null, ct);
            return organizations.Select(o => new OrganizationDto(o)).ToList();
        }

        public async Task<bool> AddUserToOrganization(Guid organizationId, Guid userId, CancellationToken ct = default)
        {
            var existing = await _organizationUserRepository.FirstOrDefault(
                ou => ou.OrganizationId == organizationId && ou.UserId == userId, ct);
            if (existing != null) return false;

            var organization = await _organizationRepository.GetById(organizationId, ct);
            if (organization == null) return false;

            var user = await _userRepository.GetById(userId, ct);
            if (user == null) return false;

            var orgUser = new OrganizationUser
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _organizationUserRepository.Add(orgUser, ct);
            return true;
        }

        public async Task<bool> RemoveUserFromOrganization(Guid organizationId, Guid userId, CancellationToken ct = default)
        {
            var orgUser = await _organizationUserRepository.FirstOrDefault(
                ou => ou.OrganizationId == organizationId && ou.UserId == userId, ct);
            if (orgUser == null) return false;

            orgUser.DeletedAt = DateTime.UtcNow;
            await _organizationUserRepository.Update(orgUser, ct);
            return true;
        }

        public async Task<List<OrganizationUserDto>> GetUsersForOrganization(Guid organizationId, CancellationToken ct = default)
        {
            var orgUsers = await _organizationUserRepository.GetWhereWithInclude(
                ou => ou.OrganizationId == organizationId && ou.DeletedAt == null, ct, ou => ou.User);
            return orgUsers.Select(ou => new OrganizationUserDto(ou)).ToList();
        }

        public async Task<PagedResponse<OrganizationDto>> GetOrganizationsPaged(PagedRequest pagedRequest, CancellationToken ct = default)
        {
            int offset = pagedRequest.PageIndex * pagedRequest.PageSize;
            var orgList = (await _organizationRepository.GetWhere(o => o.DeletedAt == null, ct))?.ToList() ?? new List<Organization>();
            int total = orgList.Count;
            var orgs = orgList
                            .Skip(offset)
                            .Take(pagedRequest.PageSize)
                            .Select(o => new OrganizationDto(o))
                            .ToList();
            return new PagedResponse<OrganizationDto>(pagedRequest.PageIndex, pagedRequest.PageSize, total, orgs);
        }
    }
}
