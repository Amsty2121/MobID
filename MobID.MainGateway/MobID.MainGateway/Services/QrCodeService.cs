using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services
{
    public class QrCodeService : IQrCodeService
    {
        private readonly IGenericRepository<QrCode> _qrCodeRepository;
        private readonly IGenericRepository<Access> _accessRepository;
        private readonly IGenericRepository<OrganizationUser> _organizationUserRepository;

        public QrCodeService(
            IGenericRepository<QrCode> qrCodeRepository,
            IGenericRepository<Access> accessRepository,
            IGenericRepository<OrganizationUser> organizationUserRepository)
        {
            _qrCodeRepository = qrCodeRepository;
            _accessRepository = accessRepository;
            _organizationUserRepository = organizationUserRepository;
        }

        public async Task<QrCodeDto> GenerateQrCode(Guid accessId, CancellationToken ct = default)
        {
            var access = await _accessRepository.GetById(accessId, ct)
                ?? throw new InvalidOperationException("Access not found.");

            var qrCode = new QrCode
            {
                Id = Guid.NewGuid(),
                AccessId = accessId,
                CreatedAt = DateTime.UtcNow
            };

            await _qrCodeRepository.Add(qrCode, ct);
            return new QrCodeDto(qrCode);
        }

        public async Task<QrCodeDto?> GetQrCodeById(Guid qrCodeId, CancellationToken ct = default)
        {
            var qrCode = await _qrCodeRepository.GetById(qrCodeId, ct);
            return qrCode == null ? null : new QrCodeDto(qrCode);
        }

        public async Task<List<QrCodeDto>> GetQrCodesForAccess(Guid accessId, CancellationToken ct = default)
        {
            var qrCodes = await _qrCodeRepository.GetWhere(qr => qr.AccessId == accessId && qr.DeletedAt == null, ct);
            return qrCodes.Select(qr => new QrCodeDto(qr)).ToList();
        }

        public async Task<bool> ValidateQrCode(Guid qrCodeId, Guid scanningUserId, CancellationToken ct = default)
        {
            var qrCode = await _qrCodeRepository.GetById(qrCodeId, ct);
            if (qrCode == null || qrCode.DeletedAt != null)
                return false;

            var access = await _accessRepository.GetById(qrCode.AccessId, ct);
            if (access == null || access.DeletedAt != null ||
                (access.ExpirationDateTime != null && access.ExpirationDateTime < DateTime.UtcNow))
            {
                return false;
            }

            if (access.RestrictToOrganizationMembers)
            {
                var membership = await _organizationUserRepository.FirstOrDefault(
                    ou => ou.OrganizationId == access.OrganizationId
                       && ou.UserId == scanningUserId,
                    ct
                );
                if (membership == null)
                    return false;
            }

            return true;
        }

        public async Task<bool> DeactivateQrCode(Guid qrCodeId, CancellationToken ct = default)
        {
            var qrCode = await _qrCodeRepository.GetById(qrCodeId, ct);
            if (qrCode == null)
                return false;
            qrCode.DeletedAt = DateTime.UtcNow;
            await _qrCodeRepository.Update(qrCode, ct);
            return true;
        }

        public async Task<PagedResponse<QrCodeDto>> GetQrCodesPaged(PagedRequest pagedRequest, CancellationToken ct = default)
        {
            int offset = pagedRequest.PageIndex * pagedRequest.PageSize;
            var qrList = (await _qrCodeRepository.GetWhere(qr => qr.DeletedAt == null, ct))?.ToList() ?? new List<QrCode>();
            int total = qrList.Count;
            var qrCodes = qrList
                                .Skip(offset)
                                .Take(pagedRequest.PageSize)
                                .Select(qr => new QrCodeDto(qr))
                                .ToList();
            return new PagedResponse<QrCodeDto>(pagedRequest.PageIndex, pagedRequest.PageSize, total, qrCodes);
        }
    }
}
