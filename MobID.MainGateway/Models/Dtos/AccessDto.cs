using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Enums;

namespace MobID.MainGateway.Models.Dtos
{
    public class AccessDto
    {
        public Guid Id { get; }
        public string Name { get; }
        public string Description { get; }

        public Guid OrganizationId { get; }
        public string OrganizationName { get; }

        public bool IsActive { get; }

        public Guid CreatedBy { get; }
        public string CreatorName { get; }

        public string AccessType { get; }
        public AccessNumberScanMode ScanMode { get; }

        public bool RestrictToOrganizationMembers { get; }

        public int? MaxUses { get; }
        public int? MaxUsersPerPass { get; }
        public int? MonthlyLimit { get; }
        public int? SubscriptionPeriodMonths { get; }
        public int? UsesPerPeriod { get; }

        public DateTime? ExpirationDateTime { get; }

        // nou
        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; }

        // existente
        public List<Guid> QrCodeIds { get; }

        // nou: utilizatorii care au fost furnizați acest acces
        public List<UserAccessDto> UserAccesses { get; }

        public AccessDto(Access a)
        {
            Id = a.Id;
            Name = a.Name;
            Description = a.Description;
            OrganizationId = a.OrganizationId;
            OrganizationName = a.Organization?.Name ?? "–";
            IsActive = a.IsActive && a.DeletedAt == null;
            CreatedBy = a.CreatedBy;
            CreatorName = a.Creator?.Username ?? "–";
            AccessType = a.AccessType?.Name ?? "–";
            ScanMode = a.ScanMode;
            RestrictToOrganizationMembers = a.RestrictToOrganizationMembers;

            MaxUses = a.MaxUses;
            MaxUsersPerPass = a.MaxUsersPerPass;
            MonthlyLimit = a.MonthlyLimit;
            SubscriptionPeriodMonths = a.SubscriptionPeriodMonths;
            UsesPerPeriod = a.UsesPerPeriod;

            ExpirationDateTime = a.ExpirationDateTime;

            CreatedAt = a.CreatedAt;
            UpdatedAt = a.UpdatedAt;

            QrCodeIds = a.QrCodes?
                .Where(q => q.DeletedAt == null)
                .Select(q => q.Id)
                .ToList()
              ?? new List<Guid>();

            UserAccesses = a.UserAccesses?
                .Where(ua => ua.DeletedAt == null)
                .Select(ua => new UserAccessDto(ua))
                .ToList()
              ?? new List<UserAccessDto>();
        }
    }
}
