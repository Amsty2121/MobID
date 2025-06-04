using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Enums;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IGenericRepository<Organization> _orgRepo;
        private readonly IGenericRepository<OrganizationUser> _orgUserRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<UserAccess> _uaRepo;
        private readonly IGenericRepository<Access> _accessRepo;
        private readonly IGenericRepository<OrganizationAccessShare> _shareRepo;

        public OrganizationService(
            IGenericRepository<Organization> orgRepo,
            IGenericRepository<OrganizationUser> orgUserRepo,
            IGenericRepository<User> userRepo,
            IGenericRepository<UserAccess> uaRepo,
            IGenericRepository<Access> accessRepo,
            IGenericRepository<OrganizationAccessShare> shareRepo)
        {
            _orgRepo = orgRepo;
            _orgUserRepo = orgUserRepo;
            _userRepo = userRepo;
            _uaRepo = uaRepo;
            _accessRepo = accessRepo;
            _shareRepo = shareRepo;
        }

        public async Task<OrganizationDto> CreateOrganizationAsync(
            OrganizationCreateReq request,
            CancellationToken ct = default)
        {
            if (await _orgRepo.FirstOrDefault(o => o.Name == request.Name, ct) != null)
                throw new InvalidOperationException($"Organization '{request.Name}' exists.");

            var owner = await _userRepo.GetById(request.OwnerId, ct)
                        ?? throw new InvalidOperationException("Owner not found.");

            var org = new Organization
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                OwnerId = request.OwnerId,
                CreatedAt = DateTime.UtcNow
            };
            await _orgRepo.Add(org, ct);

            // Add owner as member
            await AddUserToOrganizationAsync(
                org.Id,
                new OrganizationAddUserReq
                {
                    UserId = request.OwnerId,
                    Role = OrganizationUserRole.Owner.ToString()
                },
                ct
            );

            return new OrganizationDto(org);
        }

        public async Task<OrganizationDto?> GetOrganizationByIdAsync(
            Guid organizationId,
            CancellationToken ct = default)
        {
            var org = await _orgRepo.GetByIdWithInclude(
                organizationId, ct,
                o => o.Owner
            );
            return org is null
                ? null
                : new OrganizationDto(org);
        }

        public async Task<PagedResponse<OrganizationDto>> GetOrganizationsPagedAsync(
            PagedRequest request,
            CancellationToken ct = default)
        {
            var all = (await _orgRepo.GetWhereWithInclude(
                            o => o.DeletedAt == null, ct,
                            o => o.Owner))
                      .ToList();
            var total = all.Count;
            var page = all
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new OrganizationDto(o))
                .ToList();

            return new PagedResponse<OrganizationDto>(
                request.PageIndex,
                request.PageSize,
                total,
                page
            );
        }

        public async Task<List<OrganizationDto>> GetAllOrganizationsAsync(
            CancellationToken ct = default)
        {
            var list = await _orgRepo.GetWhereWithInclude(
                o => o.DeletedAt == null, ct,
                o => o.Owner
            );
            return list.Select(o => new OrganizationDto(o)).ToList();
        }

        public async Task<OrganizationDto> UpdateOrganizationAsync(
            OrganizationUpdateReq request,
            CancellationToken ct = default)
        {
            var org = await _orgRepo.GetById(request.OrganizationId, ct)
                      ?? throw new InvalidOperationException("Organization not found.");

            var isChanged = false;

            // rename?
            if (!string.IsNullOrWhiteSpace(request.Name)
                && org.Name != request.Name)
            {
                if (await _orgRepo.FirstOrDefault(
                        o => o.Name == request.Name && o.Id != org.Id,
                        ct) != null)
                    throw new InvalidOperationException($"Organization '{request.Name}' exists.");

                org.Name = request.Name;
                isChanged = true;
            }

            // change owner?
            if (request.OwnerId.HasValue && request.OwnerId != Guid.Empty
                && request.OwnerId != org.OwnerId)
            {
                var newOwnerRel = await _orgUserRepo.FirstOrDefault(
                    ou => ou.OrganizationId == org.Id
                       && ou.UserId == request.OwnerId
                       && ou.DeletedAt == null,
                    ct)
                    ?? throw new InvalidOperationException("New owner must be a member.");

                var oldOwnerRel = await _orgUserRepo.FirstOrDefault(
                    ou => ou.OrganizationId == org.Id
                       && ou.UserId == org.OwnerId
                       && ou.DeletedAt == null,
                    ct)
                    ?? throw new InvalidOperationException("Previous owner relation not found.");

                newOwnerRel.Role = OrganizationUserRole.Owner;
                newOwnerRel.UpdatedAt = DateTime.UtcNow;
                await _orgUserRepo.Update(newOwnerRel, ct);

                oldOwnerRel.Role = OrganizationUserRole.Admin;
                oldOwnerRel.UpdatedAt = DateTime.UtcNow;
                await _orgUserRepo.Update(oldOwnerRel, ct);

                org.OwnerId = request.OwnerId.Value;
                isChanged = true;
            }

            if (isChanged)
            {
                org.UpdatedAt = DateTime.UtcNow;
                await _orgRepo.Update(org, ct);
            }

            return new OrganizationDto(org);
        }

        public async Task<bool> DeactivateOrganizationAsync(
            Guid organizationId,
            CancellationToken ct = default)
        {
            var org = await _orgRepo.GetById(organizationId, ct);
            if (org == null) return false;
            org.DeletedAt = DateTime.UtcNow;
            await _orgRepo.Update(org, ct);
            return true;
        }

        public async Task<bool> AddUserToOrganizationAsync(
            Guid organizationId,
            OrganizationAddUserReq request,
            CancellationToken ct = default)
        {
            var org = await _orgRepo.GetById(organizationId, ct)
                       ?? throw new InvalidOperationException("Organization not found.");
            var user = await _userRepo.GetById(request.UserId, ct)
                       ?? throw new InvalidOperationException("User not found.");

            // already a member?
            if (await _orgUserRepo.FirstOrDefault(
                    ou => ou.OrganizationId == organizationId
                       && ou.UserId == request.UserId,
                    ct) != null)
                return false;

            // parse the role
            var role = Enum.TryParse<OrganizationUserRole>(request.Role, out var r)
                       ? r
                       : OrganizationUserRole.Member;

            var orgUser = new OrganizationUser
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                UserId = request.UserId,
                Role = role,
                CreatedAt = DateTime.UtcNow
            };
            await _orgUserRepo.Add(orgUser, ct);

            // automatically grant this user all existing accesses
            var accesses = await _accessRepo.GetWhere(
                a => a.OrganizationId == organizationId && a.DeletedAt == null,
                ct
            );
            foreach (var a in accesses)
            {
                if (await _uaRepo.FirstOrDefault(ua =>
                        ua.UserId == request.UserId &&
                        ua.AccessId == a.Id &&
                        ua.DeletedAt == null,
                        ct) != null)
                    continue;

                var ua = new UserAccess
                {
                    Id = Guid.NewGuid(),
                    UserId = request.UserId,
                    AccessId = a.Id,
                    CreatedAt = DateTime.UtcNow
                };
                await _uaRepo.Add(ua, ct);
            }

            return true;
        }

        public async Task<bool> RemoveUserFromOrganizationAsync(
            Guid organizationId,
            Guid userId,
            CancellationToken ct = default)
        {
            var ou = await _orgUserRepo.FirstOrDefault(
                x => x.OrganizationId == organizationId && x.UserId == userId,
                ct);
            if (ou == null) return false;
            ou.DeletedAt = DateTime.UtcNow;
            await _orgUserRepo.Update(ou, ct);
            return true;
        }

        public async Task<List<OrganizationUserDto>> GetUsersForOrganizationAsync(
            Guid organizationId,
            CancellationToken ct = default)
        {
            var list = await _orgUserRepo.GetWhereWithInclude(
                ou => ou.OrganizationId == organizationId && ou.DeletedAt == null,
                ct, ou => ou.User
            );
            return list.Select(ou => new OrganizationUserDto(ou)).ToList();
        }

        /// <inheritdoc/>
        public async Task<List<AccessDto>> GetOrganizationAccessesAsync(
            Guid organizationId,
            CancellationToken ct = default)
        {
            var list = await _accessRepo.GetWhereWithInclude(
                a => a.OrganizationId == organizationId && a.DeletedAt == null,
                ct,
                a => a.Organization,
                a => a.CreatedByUser,
                a => a.AccessType,
                a => a.QrCodes
            );
            return list.Select(a => new AccessDto(a)).ToList();
        }

        /// <inheritdoc/>
        public async Task<List<OrganizationAccessShareDto>> GetAccessesSharedToOrganizationAsync(
            Guid organizationId,
            CancellationToken ct = default)
        {
            var shares = await _shareRepo.GetWhereWithInclude(
                s => s.TargetOrganizationId == organizationId && s.DeletedAt == null,
                ct,
                s => s.Access,
                s => s.SourceOrganization,
                s => s.TargetOrganization
            );
            return shares
                .Select(s => new OrganizationAccessShareDto(s))
                .ToList();
        }

        /// <inheritdoc/>
        /// <inheritdoc/>
        public async Task<List<AccessDto>> GetAllOrganizationAccessesAsync(
            Guid organizationId,
            CancellationToken ct = default)
        {
            // 1️⃣ Accese proprii
            var own = await _accessRepo.GetWhereWithInclude(
                a => a.OrganizationId == organizationId && a.DeletedAt == null,
                ct,
                a => a.Organization,
                a => a.CreatedByUser,
                a => a.AccessType,
                a => a.QrCodes
            );

            // 2️⃣ Accese partajate *către* această organizație
            var shareTargets = await _shareRepo.GetWhere(
                s => s.TargetOrganizationId == organizationId && s.DeletedAt == null,
                ct
            );
            var sharedIds = shareTargets.Select(s => s.AccessId).Distinct().ToList();

            if (!sharedIds.Any())
            {
                return own.Select(a => new AccessDto(a)).ToList();
            }

            var shared = await _accessRepo.GetWhereWithInclude(
                a => sharedIds.Contains(a.Id) && a.DeletedAt == null,
                ct,
                a => a.Organization,
                a => a.CreatedByUser,
                a => a.AccessType,
                a => a.QrCodes
            );

            // 3️⃣ Combinăm și construim DTO-urile
            var combined = own
                .Union(shared)
                .GroupBy(a => a.Id)
                .Select(g => g.First())
                .Select(a => new AccessDto(a))
                .ToList();

            return combined;
        }
    }
}