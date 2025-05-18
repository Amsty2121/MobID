using MobID.MainGateway.Models.Dtos;
using MobID.MainGateway.Models.Dtos.Req;
using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Enums;
using MobID.MainGateway.Repo.Interfaces;
using MobID.MainGateway.Services.Interfaces;

namespace MobID.MainGateway.Services
{
    public class AccessService : IAccessService
    {
        private readonly IGenericRepository<Access> _accessRepo;
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGenericRepository<AccessType> _accessTypeRepo;
        private readonly IGenericRepository<Organization> _orgRepo;

        public AccessService(
            IGenericRepository<Access> accessRepo,
            IGenericRepository<User> userRepo,
            IGenericRepository<AccessType> accessTypeRepo,
            IGenericRepository<Organization> orgRepo)
        {
            _accessRepo = accessRepo;
            _userRepo = userRepo;
            _accessTypeRepo = accessTypeRepo;
            _orgRepo = orgRepo;
        }

        public async Task<AccessDto> CreateAccess(AccessCreateReq req, Guid creatorId, CancellationToken ct = default)
        {
            var at = await _accessTypeRepo.GetById(req.AccessTypeId, ct)
                ?? throw new InvalidOperationException("Access type not found.");

            // validări articol
            switch (at.Name)
            {
                case "OneUse":
                    if (!req.MaxUsersPerPass.HasValue)
                        throw new InvalidOperationException("MaxUsersPerPass e obligatoriu pentru OneUse.");
                    break;
                case "MultiUse":
                    if (!req.MaxUses.HasValue || !req.MaxUsersPerPass.HasValue)
                        throw new InvalidOperationException("MaxUses și MaxUsersPerPass sunt obligatorii pentru MultiUse.");
                    break;
                case "Subscription":
                    if (!req.MonthlyLimit.HasValue || !req.SubscriptionPeriodMonths.HasValue)
                        throw new InvalidOperationException("MonthlyLimit și SubscriptionPeriodMonths sunt obligatorii pentru Subscription.");
                    break;
                case "IdentityConfirm":
                    // nici o validare suplimentară
                    break;
                default:
                    throw new InvalidOperationException("Tip de acces necunoscut.");
            }

            var org = await _orgRepo.GetById(req.OrganizationId, ct)
                ?? throw new InvalidOperationException("Organization not found.");

            var access = new Access
            {
                Id = Guid.NewGuid(),
                OrganizationId = org.Id,
                CreatedBy = creatorId,
                AccessTypeId = at.Id,
                Description = req.Description,
                ExpirationDateTime = req.ExpirationDate.HasValue
                    ? DateTime.SpecifyKind(req.ExpirationDate.Value, DateTimeKind.Utc)
                    : (DateTime?)null,
                RestrictToOrganizationMembers = req.RestrictToOrganizationMembers,
                ScanMode = Enum.Parse<AccessNumberScanMode>(req.ScanMode),
                MaxUses = req.MaxUses,
                MaxUsersPerPass = req.MaxUsersPerPass,
                MonthlyLimit = req.MonthlyLimit,
                SubscriptionPeriodMonths = req.SubscriptionPeriodMonths,
                UpdatedAt = DateTime.UtcNow
            };

            await _accessRepo.Add(access, ct);
            return new AccessDto(access);
        }


        public async Task<AccessDto?> GetAccessById(Guid accessId, CancellationToken ct = default)
        {
            var a = await _accessRepo.GetByIdWithInclude(
                accessId, ct,
                x => x.Organization,
                x => x.Creator,
                x => x.AccessType,
                x => x.QrCodes
            );
            return a is null ? null : new AccessDto(a);
        }

        public async Task<List<AccessDto>> GetAccessesForOrganization(Guid organizationId, CancellationToken ct = default)
        {
            var list = await _accessRepo.GetWhereWithInclude(
                x => x.OrganizationId == organizationId && x.DeletedAt == null,
                ct,
                x => x.Organization,
                x => x.Creator,
                x => x.AccessType,
                x => x.QrCodes
            );
            return list.Select(x => new AccessDto(x)).ToList();
        }

        public async Task<bool> DeactivateAccess(Guid accessId, CancellationToken ct = default)
        {
            var a = await _accessRepo.GetById(accessId, ct);
            if (a == null) return false;
            a.DeletedAt = DateTime.UtcNow;
            await _accessRepo.Update(a, ct);
            return true;
        }

        public async Task<PagedResponse<AccessDto>> GetAccessesPaged(PagedRequest pagedRequest, CancellationToken ct = default)
        {
            var all = (await _accessRepo.GetWhereWithInclude(x => x.DeletedAt == null, ct, x => x.AccessType, x => x.Creator))?.ToList()
                      ?? new List<Access>();
            int total = all.Count;
            var page = all
                .Skip(pagedRequest.PageIndex * pagedRequest.PageSize)
                .Take(pagedRequest.PageSize)
                .Select(x => new AccessDto(x))
                .ToList();

            return new PagedResponse<AccessDto>(pagedRequest.PageIndex, pagedRequest.PageSize, total, page);
        }
    }
}
