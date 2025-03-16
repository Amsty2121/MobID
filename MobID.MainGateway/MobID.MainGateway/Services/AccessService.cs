using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos.Rsp;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace MobID.MainGateway.Services
{
    public class AccessService : IAccessService
    {
        private readonly IGenericRepository<Access> _accessRepository;
        private readonly IGenericRepository<AccessType> _accessTypeRepository;
        private readonly IGenericRepository<Organization> _organizationRepository;
        private readonly IGenericRepository<QrCode> _qrCodeRepository;

        public AccessService(
            IGenericRepository<Access> accessRepository,
            IGenericRepository<AccessType> accessTypeRepository,
            IGenericRepository<Organization> organizationRepository,
            IGenericRepository<QrCode> qrCodeRepository)
        {
            _accessRepository = accessRepository;
            _accessTypeRepository = accessTypeRepository;
            _organizationRepository = organizationRepository;
            _qrCodeRepository = qrCodeRepository;
        }

        public async Task<AccessDto> CreateAccess(AccessCreateReq request, CancellationToken ct = default)
        {
            var accessType = await _accessTypeRepository.GetById(request.AccessTypeId, ct)
                ?? throw new InvalidOperationException("Access type not found.");

            var organization = await _organizationRepository.GetById(request.OrganizationId, ct)
                ?? throw new InvalidOperationException("Organization not found.");

            var access = new Access
            {
                Id = Guid.NewGuid(),
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
    }
}
