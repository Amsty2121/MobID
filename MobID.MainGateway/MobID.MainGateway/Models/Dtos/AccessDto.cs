using MobID.MainGateway.Models.Entities;
using MobID.MainGateway.Models.Enums;

namespace MobID.MainGateway.Models.Dtos
{
    public class AccessDto
    {
        public Guid Id { get; }
        public Guid OrganizationId { get; }
        public string OrganizationName { get; }
        public bool IsActive { get; }
        public Guid CreatedBy { get; }
        public string CreatorName { get; }
        public string AccessType { get; }
        public ScanMode ScanMode { get; }
        public int? MaxUses { get; }
        public int? MaxUsersPerPass { get; }
        public int? MonthlyLimit { get; }
        public int? TotalUses { get; }
        public int? MaxUsersPerUsage { get; }
        public DateTime? ExpirationDateTime { get; }
        public List<Guid> QrCodeIds { get; }
        public bool IsDeleted { get; }
        public AccessDto(Access access)
        {
            Id = access.Id;
            OrganizationId = access.OrganizationId;
            OrganizationName = access.Organization?.Name ?? "Unknown";
            IsActive = access.IsActive && access.DeletedAt == null;
            CreatedBy = access.CreatedBy;
            CreatorName = access.Creator?.Username ?? "Unknown";
            AccessType = access.AccessType?.Name ?? "Unknown";
            ScanMode = access.ScanMode;
            MaxUses = access.MaxUses;
            MaxUsersPerPass = access.MaxUsersPerPass;
            MonthlyLimit = access.MonthlyLimit;
            TotalUses = access.TotalUses;
            MaxUsersPerUsage = access.MaxUsersPerUsage;
            ExpirationDateTime = access.ExpirationDateTime;
            QrCodeIds = access.QrCodes?.Select(qr => qr.Id).ToList() ?? new List<Guid>();
            IsDeleted = access.DeletedAt == null;
        }
    }
}
