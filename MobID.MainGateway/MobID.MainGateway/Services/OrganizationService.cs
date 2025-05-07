using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            // 1. Nume unic
            if (await _organizationRepository.FirstOrDefault(o => o.Name == request.Name, ct) != null)
                throw new InvalidOperationException($"O organizație cu numele '{request.Name}' există deja.");

            // 2. Proprietar valid
            var owner = await _userRepository.GetById(request.OwnerId, ct);
            if (owner == null)
                throw new InvalidOperationException($"Utilizatorul cu ID '{request.OwnerId}' nu există.");

            // 3. Creare organizație
            var organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                OwnerId = request.OwnerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _organizationRepository.Add(organization, ct);

            // 4. Adăugăm owner ca membru cu rol Owner
            await AddUserToOrganization(
                organization.Id,
                organization.OwnerId,
                OrganizationUserRole.Owner,
                ct);

            return new OrganizationDto(organization);
        }

        public async Task<OrganizationDto?> GetOrganizationById(Guid organizationId, CancellationToken ct = default)
        {
            var org = await _organizationRepository.GetByIdWithInclude(
                organizationId, ct, o => o.OrganizationUsers);
            return org == null ? null : new OrganizationDto(org);
        }

        public async Task<List<OrganizationDto>> GetAllOrganizations(CancellationToken ct = default)
        {
            var orgs = await _organizationRepository.GetWhereWithInclude(o => o.DeletedAt == null, ct, x => x.Owner);
            return orgs.Select(o => new OrganizationDto(o)).ToList();
        }

        public async Task<bool> AddUserToOrganization(
            Guid organizationId,
            Guid userId,
            OrganizationUserRole role = OrganizationUserRole.Member,
            CancellationToken ct = default)
        {
            // 1. Organizație existență
            var org = await _organizationRepository.GetById(organizationId, ct);
            if (org == null)
                throw new InvalidOperationException($"Organizația cu ID '{organizationId}' nu există.");

            // 2. Utilizator existență
            var user = await _userRepository.GetById(userId, ct);
            if (user == null)
                throw new InvalidOperationException($"Utilizatorul cu ID '{userId}' nu există.");

            // 3. Verificăm duplicat
            var existing = await _organizationUserRepository.FirstOrDefault(
                ou => ou.OrganizationId == organizationId && ou.UserId == userId, ct);
            if (existing != null)
                return false;

            // 4. Creăm membership
            var orgUser = new OrganizationUser
            {
                Id = Guid.NewGuid(),
                OrganizationId = organizationId,
                UserId = userId,
                Role = role,
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
            if (orgUser == null)
                return false;

            orgUser.DeletedAt = DateTime.UtcNow;
            await _organizationUserRepository.Update(orgUser, ct);
            return true;
        }

        public async Task<List<OrganizationUserDto>> GetUsersForOrganization(Guid organizationId, CancellationToken ct = default)
        {
            var orgUsers = await _organizationUserRepository.GetWhereWithInclude(
                ou => ou.OrganizationId == organizationId && ou.DeletedAt == null,
                ct,
                ou => ou.User);
            return orgUsers.Select(ou => new OrganizationUserDto(ou)).ToList();
        }

        public async Task<PagedResponse<OrganizationDto>> GetOrganizationsPaged(PagedRequest pagedRequest, CancellationToken ct = default)
        {
            int offset = pagedRequest.PageIndex * pagedRequest.PageSize;
            var allOrgs = (await _organizationRepository.GetWhereWithInclude(
                                o => o.DeletedAt == null,
                                ct,
                                o => o.Owner))
                          .ToList();
            int total = allOrgs.Count;
            var page = allOrgs
                        .Skip(offset)
                        .Take(pagedRequest.PageSize)
                        .Select(o => new OrganizationDto(o))
                        .ToList();
            return new PagedResponse<OrganizationDto>(
                pagedRequest.PageIndex,
                pagedRequest.PageSize,
                total,
                page);
        }

        public async Task<bool> DeleteOrganization(Guid organizationId, CancellationToken ct = default)
        {
            var org = await _organizationRepository.GetById(organizationId, ct);
            if (org == null)
                return false;

            org.DeletedAt = DateTime.UtcNow;
            await _organizationRepository.Update(org, ct);
            return true;
        }

        public async Task<OrganizationDto> UpdateOrganization(OrganizationUpdateReq request, CancellationToken ct = default)
        {
            var org = await _organizationRepository.GetById(request.OrganizationId, ct);
            if (org == null)
                throw new InvalidOperationException("Organizație inexistentă.");

            // Validăm unicitatea numelui
            if (!string.IsNullOrWhiteSpace(request.Name))
            {
                var duplicate = await _organizationRepository.FirstOrDefault(
                    o => o.Name == request.Name && o.Id != request.OrganizationId, ct);
                if (duplicate != null)
                    throw new InvalidOperationException($"Există deja o organizație cu numele '{request.Name}'.");
                org.Name = request.Name;
            }

            // Validăm owner nou, dacă e specificat
            if (request.OwnerId.HasValue)
            {
                var newOwner = await _userRepository.GetById(request.OwnerId.Value, ct);
                if (newOwner == null)
                    throw new InvalidOperationException($"Utilizatorul cu ID '{request.OwnerId}' nu există.");
                org.OwnerId = request.OwnerId.Value;

                // Actualizăm rolul vechiului owner la Member
                var oldOwner = await _organizationUserRepository.FirstOrDefault(
                    ou => ou.OrganizationId == org.Id && ou.UserId != request.OwnerId.Value, ct);
                if (oldOwner != null)
                {
                    oldOwner.Role = OrganizationUserRole.Member;
                    await _organizationUserRepository.Update(oldOwner, ct);
                }

                // Adăugăm noul owner ca Owner, dacă nu există deja
                var existingNewOwner = await _organizationUserRepository.FirstOrDefault(
                    ou => ou.OrganizationId == org.Id && ou.UserId == request.OwnerId.Value, ct);
                if (existingNewOwner != null)
                {
                    existingNewOwner.Role = OrganizationUserRole.Owner;
                    await _organizationUserRepository.Update(existingNewOwner, ct);
                }
                else
                {
                    await AddUserToOrganization(
                        org.Id,
                        request.OwnerId.Value,
                        OrganizationUserRole.Owner,
                        ct);
                }
            }

            org.UpdatedAt = DateTime.UtcNow;
            await _organizationRepository.Update(org, ct);

            return new OrganizationDto(org);
        }
    }
}
