using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Dtos.Rsp;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Enums;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services
{
    public class QrCodeService : IQrCodeService
    {
        private readonly IGenericRepository<QrCode> _qrRepo;
        private readonly IGenericRepository<Access> _accessRepo;
        private readonly IGenericRepository<User> _userRepo;

        public QrCodeService(
            IGenericRepository<QrCode> qrRepo,
            IGenericRepository<Access> accessRepo,
            IGenericRepository<User> userRepo)
        {
            _qrRepo = qrRepo;
            _accessRepo = accessRepo;
            _userRepo = userRepo;
        }

        public async Task<QrCodeDto> CreateQrCodeAsync(
            QrCodeGenerateReq req,
            CancellationToken ct = default)
        {
            // ensure the Access + its OrganizationId is loaded
            var access = await _accessRepo.GetByIdWithInclude(
                req.AccessId, ct,
                a => a.AccessType,
                a => a.Organization
            ) ?? throw new InvalidOperationException("Access not found.");

            var qr = new QrCode
            {
                Id = Guid.NewGuid(),
                Description = req.Description ?? $"QR for {access.Name}",
                Type = QrCodeType.AccessConfirm,
                AccessId = req.AccessId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                DeletedAt = null
            };

            await _qrRepo.Add(qr, ct);

            // stitch back the Access so the DTO can see OrganizationId
            qr.Access = access;

            return new QrCodeDto(qr);
        }

        public async Task<QrCodeDto?> GetQrCodeByIdAsync(
            Guid qrCodeId,
            CancellationToken ct = default)
        {
            var qr = await _qrRepo.FirstOrDefaultWithInclude(
                q => q.Id == qrCodeId && q.DeletedAt == null,
                ct,
                q => q.Access
            );

            return qr is null ? null : new QrCodeDto(qr);
        }

        public async Task<List<QrCodeDto>> GetQrCodesForAccessAsync(
            Guid accessId,
            CancellationToken ct = default)
        {
            var qrs = await _qrRepo.GetWhereWithInclude(
                q => q.AccessId == accessId && q.DeletedAt == null,
                ct,
                q => q.Access
            );

            return qrs.Select(q => new QrCodeDto(q)).ToList();
        }

        public async Task<PagedResponse<QrCodeDto>> GetQrCodesPagedAsync(
            PagedRequest request,
            CancellationToken ct = default)
        {
            var all = (await _qrRepo.GetWhereWithInclude(
                q => q.DeletedAt == null,
                ct,
                q => q.Access
            )).ToList();

            var total = all.Count;
            var page = all
                .Skip(request.PageIndex * request.PageSize)
                .Take(request.PageSize)
                .Select(q => new QrCodeDto(q))
                .ToList();

            return new PagedResponse<QrCodeDto>(
                request.PageIndex,
                request.PageSize,
                total,
                page
            );
        }

        public async Task<bool> DeactivateQrCodeAsync(
            Guid qrCodeId,
            CancellationToken ct = default)
        {
            var qr = await _qrRepo.GetById(qrCodeId, ct);
            if (qr == null) return false;

            qr.DeletedAt = DateTime.UtcNow;
            qr.UpdatedAt = DateTime.UtcNow;
            await _qrRepo.Update(qr, ct);
            return true;
        }

        public async Task<AccessValidationRsp> ValidateQrCodeAsync(
            Guid qrCodeId,
            Guid scannedByUserId,
            CancellationToken ct = default)
        {
            var qr = await _qrRepo.FirstOrDefaultWithInclude(
                q => q.Id == qrCodeId && q.DeletedAt == null,
                ct,
                q => q.Access
            );

            if (qr == null)
            {
                return new AccessValidationRsp(false, "QR code is invalid or expired.");
            }

            // ... any further validation rules ...

            return new AccessValidationRsp(true, "Access granted");
        }
    }
}
