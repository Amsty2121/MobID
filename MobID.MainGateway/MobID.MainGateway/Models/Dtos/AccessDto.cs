using System;
using System.Collections.Generic;
using System.Linq;
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


        // Limitări generale
        public int? TotalUseLimit { get; }
        public int? UseLimitPerPeriod { get; }
        public TimeSpan? SubscriptionPeriod { get; }

        public DateTime? ExpirationDateTime { get; }

        public bool IsRestrictedToOrgMembers { get; }
        public bool IsRestrictedToOrganizationShare { get; }

        public DateTime CreatedAt { get; }
        public DateTime UpdatedAt { get; }

        // ID-urile codurilor QR aferente
        public List<Guid> QrCodeIds { get; }

        // Utilizatorii cărora li s-a acordat acest acces
        public List<UserAccessDto> UserAccesses { get; }

        public AccessDto(Access a)
        {
            Id = a.Id;
            Name = a.Name;
            Description = a.Description;

            OrganizationId = a.OrganizationId;
            OrganizationName = a.Organization?.Name ?? "–";

            IsActive = a.DeletedAt == null;

            CreatedBy = a.CreatedByUserId;
            CreatorName = a.CreatedByUser?.Username ?? "–";

            AccessType = a.AccessType?.Name ?? "–";

            TotalUseLimit = a.TotalUseLimit;
            UseLimitPerPeriod = a.UseLimitPerPeriod;
            SubscriptionPeriod = a.SubscriptionPeriod;

            ExpirationDateTime = a.ExpirationDateTime;

            IsRestrictedToOrgMembers = a.IsRestrictedToOrgMembers;
            IsRestrictedToOrganizationShare = a.IsRestrictedToOrganizationShare;

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
