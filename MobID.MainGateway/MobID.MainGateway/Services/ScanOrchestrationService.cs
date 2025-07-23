using System;
using System.Linq;
using System.Text;
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
    public class ScanOrchestrationService : IScanOrchestrationService
    {
        private readonly IGenericRepository<Organization> _orgRepo;
        private readonly IGenericRepository<OrganizationAccessShare> _orgShareRepo;
        private readonly IGenericRepository<Access> _accessRepo;
        private readonly IGenericRepository<QrCode> _qrRepo;
        private readonly IGenericRepository<Scan> _scanRepo;
        private readonly IGenericRepository<OrganizationUser> _orgUserRepo;
        private readonly IGenericRepository<UserAccess> _userAccessRepo;
        private readonly IGenericRepository<User> _userRepo;

        public ScanOrchestrationService(
            IGenericRepository<Organization> orgRepo,
            IGenericRepository<Access> accessRepo,
            IGenericRepository<QrCode> qrRepo,
            IGenericRepository<Scan> scanRepo,
            IGenericRepository<OrganizationUser> orgUserRepo,
            IGenericRepository<UserAccess> userAccessRepo,
            IGenericRepository<User> userRepo,
            IGenericRepository<OrganizationAccessShare> orgShareRepo)
        {
            _orgRepo = orgRepo;
            _accessRepo = accessRepo;
            _qrRepo = qrRepo;
            _scanRepo = scanRepo;
            _orgUserRepo = orgUserRepo;
            _userAccessRepo = userAccessRepo;
            _userRepo = userRepo;
            _orgShareRepo = orgShareRepo;
        }

        /// <summary>
        /// Încearcă să proceseze scanarea codului QR.
        /// Returnează un tuple cu succesul operației și DTO-ul scanării (sau null).
        /// Toate validările și acțiunile interne folosesc valori booleene, nu excepții.
        /// </summary>
        public async Task<ScanDto?> HandleQrScanAsync(
            string qrRawValue,
            Guid scanningUserId,
            CancellationToken ct = default)
        {
            // 0. Încearcă să obții userul
            var user = await _userRepo.GetById(scanningUserId, ct);
            if (user is null)
                return null;

            // 1. Parsează raw (Org:Access:Qr:Type)
            if (!TryParseRaw(qrRawValue, out var orgId, out var accessId, out var qrCodeId, out var qrType))
                return null;

            // 2. Validări entități & relații
            if (!await TryEnsureOrgExists(orgId, ct)) return null;
            if (!await TryEnsureAccessBelongs(orgId, accessId, ct)) return null;
            if (!await TryEnsureQrBelongs(qrCodeId, accessId, qrType, ct)) return null;

            // 3. Înregistrează scanarea
            var scanEntity = new Scan
            {
                Id = Guid.NewGuid(),
                QrCodeId = qrCodeId,
                ScannedByUserId = scanningUserId,
                ScannedForUserId = scanningUserId,
                IsSuccessfull = true
            };

            // 4. Procesează acțiunea conform tipului QR
            if (!await TryProcessQrType(qrType, orgId, accessId, qrCodeId, scanningUserId, ct))
                scanEntity.IsSuccessfull = false;

            await _scanRepo.Add(scanEntity, ct);

            // 5. Returnează DTO
            return scanEntity.IsSuccessfull ? new ScanDto(scanEntity) : null;
        }

        //–– Parse & validate raw string
        private bool TryParseRaw(
            string raw,
            out Guid orgId,
            out Guid accessId,
            out Guid qrCodeId,
            out QrCodeType qrType)
        {
            orgId = accessId = qrCodeId = Guid.Empty;
            qrType = default;

            // 1) decodăm Base64
            string decoded;
            try
            {
                var bytes = Convert.FromBase64String(raw);
                decoded = Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return false;
            }

            // 2) split și parse Guid/enum
            var parts = decoded.Split(':');
            if (parts.Length != 4) return false;

            if (!Guid.TryParse(parts[0], out orgId)) return false;
            if (!Guid.TryParse(parts[1], out accessId)) return false;
            if (!Guid.TryParse(parts[2], out qrCodeId)) return false;
            if (!Enum.TryParse(parts[3], true, out qrType)) return false;

            return true;
        }

        //–– Verify Organization exists
        private async Task<bool> TryEnsureOrgExists(Guid orgId, CancellationToken ct)
        {
            return await _orgRepo.GetById(orgId, ct) is not null;
        }

        //–– Verify Access exists & belongs to org
        private async Task<bool> TryEnsureAccessBelongs(
            Guid orgId,
            Guid accessId,
            CancellationToken ct)
        {
            var access = await _accessRepo.GetById(accessId, ct);
            return access is not null && access.OrganizationId == orgId;
        }

        //–– Verify QR exists, belongs & type matches
        private async Task<bool> TryEnsureQrBelongs(
            Guid qrCodeId,
            Guid accessId,
            QrCodeType qrType,
            CancellationToken ct)
        {
            var qr = await _qrRepo.GetById(qrCodeId, ct);
            return qr is not null
                && qr.AccessId == accessId
                && qr.Type == qrType;
        }

        //–– Process QR action by type (return false if validation fails)
        private async Task<bool> TryProcessQrType(
            QrCodeType qrType,
            Guid orgId,
            Guid accessId,
            Guid qrCodeId,
            Guid userId,
            CancellationToken ct)
        {
            return qrType switch
            {
                QrCodeType.InviteToOrganization => await TryInviteAsync(orgId, userId, ct),
                QrCodeType.ShareAccess => await TryShareAccessAsync(accessId, userId, ct),
                QrCodeType.AccessConfirm => await TryAccessConfirmAsync(orgId, accessId, qrCodeId, userId, ct),
                _ => false
            };
        }

        //–– Invite: doar dacă nu există deja membru
        private async Task<bool> TryInviteAsync(
            Guid orgId,
            Guid userId,
            CancellationToken ct)
        {
            var existing = await _orgUserRepo.FirstOrDefault(
                ou => ou.OrganizationId == orgId && ou.UserId == userId,
                ct);
            if (existing != null) return true;

            await _orgUserRepo.Add(new OrganizationUser
            {
                Id = Guid.NewGuid(),
                OrganizationId = orgId,
                UserId = userId,
                Role = OrganizationUserRole.Member
            }, ct);
            return true;
        }

        //–– ShareAccess: doar dacă nu are deja acces
        private async Task<bool> TryShareAccessAsync(
            Guid accessId,
            Guid userId,
            CancellationToken ct)
        {
            var existing = await _userAccessRepo.FirstOrDefault(
                ua => ua.AccessId == accessId && ua.UserId == userId,
                ct);
            if (existing != null) return true;

            await _userAccessRepo.Add(new UserAccess
            {
                Id = Guid.NewGuid(),
                AccessId = accessId,
                UserId = userId
            }, ct);
            return true;
        }

        //–– AccessConfirm: toate regulile de validare, return true doar dacă e permis
        private async Task<bool> TryAccessConfirmAsync(
            Guid orgId,
            Guid accessId,
            Guid qrCodeId,
            Guid userId,
            CancellationToken ct)
        {
            var access = await _accessRepo.GetById(accessId, ct);
            if (access == null || !access.IsActive) return false;

            // Single-scan?
            if (!access.IsMultiScan)
            {
                var count = await _scanRepo.CountWhere(s => s.QrCodeId == qrCodeId);
                if (count > 1) return false;
            }

            // TotalUseLimit?
            if (access.TotalUseLimit.HasValue)
            {
                var total = await _scanRepo.CountWhere(s => s.QrCodeId == qrCodeId);
                if (total > access.TotalUseLimit.Value) return false;
            }

            // UseLimitPerPeriod + SubscriptionPeriodMonths?
            if (access.UseLimitPerPeriod.HasValue && access.SubscriptionPeriodMonths.HasValue)
            {
                var windowStart = DateTime.UtcNow.AddMonths(-access.SubscriptionPeriodMonths.Value);
                var period = await _scanRepo.CountWhere(
                    s => s.QrCodeId == qrCodeId && s.CreatedAt >= windowStart);
                if (period > access.UseLimitPerPeriod.Value) return false;
            }

            // RestrictToOrgMembers?
            if (access.RestrictToOrgMembers)
            {
                var member = await _orgUserRepo.FirstOrDefault(
                    ou => ou.OrganizationId == orgId && ou.UserId == userId, ct);
                if (member == null) return false;
            }

            // RestrictToOrgSharing?
            if (access.RestrictToOrgSharing)
            {
                var shared = await _orgShareRepo.FirstOrDefault(
                    os => os.AccessId == accessId && os.TargetOrganizationId == orgId, ct);
                if (shared == null) return false;
            }

            // Confirmare finală: direct sau prin organizație
            var hasAccess = (await GetAllUserAccessesAsync(userId)).Select(x=>x.Id).Contains(accessId);

            return hasAccess;
        }

        public async Task<List<Access>> GetAllUserAccessesAsync(Guid userId, CancellationToken ct = default)
        {
            // 1️⃣ Direct user‐assigned accesses
            var userAccesses = await _userAccessRepo.GetWhereWithInclude(
                ua => ua.UserId == userId && ua.DeletedAt == null,
                ct,
                ua => ua.Access,            // include the Access navigation
                ua => ua.Access.AccessType,
                ua => ua.Access.Organization,
                ua => ua.Access.CreatedByUser,
                ua => ua.Access.QrCodes
            );
            var direct = userAccesses.Select(ua => ua.Access);

            var memberships = await _orgUserRepo.GetWhere(
                ou => ou.UserId == userId && ou.DeletedAt == null,
                ct
            );
            var orgIds = memberships.Select(ou => ou.OrganizationId).Distinct().ToList();

            var orgAccesses = orgIds.Any()
                ? await _accessRepo.GetWhereWithInclude(
                      a => orgIds.Contains(a.OrganizationId) && a.DeletedAt == null,
                      ct,
                      a => a.AccessType,
                      a => a.Organization,
                      a => a.CreatedByUser,
                      a => a.QrCodes
                  )
                : new List<Access>();

            var shares = await _orgShareRepo.GetWhere(
                s => orgIds.Contains(s.TargetOrganizationId) && s.DeletedAt == null,
                ct
            );
            var sharedIds = shares.Select(s => s.AccessId).Distinct().ToList();
            var shared = sharedIds.Any()
                ? await _accessRepo.GetWhereWithInclude(
                      a => sharedIds.Contains(a.Id) && a.DeletedAt == null,
                      ct,
                      a => a.AccessType,
                      a => a.Organization,
                      a => a.CreatedByUser,
                      a => a.QrCodes
                  )
                : new List<Access>();

            var all = direct
                .Union(orgAccesses)
                .Union(shared)
                .GroupBy(a => a.Id)
                .Select(g => g.First());

            return all.ToList();
        }
    }
}
