using MobID.MainGateway.Models.Enums;

namespace MobID.MainGateway.Models.Entities
{
    public class Access : IBaseEntity
    {
        public Guid Id { get; set; }

        public Guid OrganizationId { get; set; }
        public Organization Organization { get; set; }

        public bool IsActive { get; set; } = true;

        public Guid CreatedBy { get; set; }
        public User Creator { get; set; }


        public Guid AccessTypeId { get; set; }
        public AccessType AccessType { get; set; }


        public ScanMode ScanMode { get; set; } // Cine scanează și cum funcționează accesul

        public int? MaxUses { get; set; } // Ex: 8 intrări (null = nelimitat)
        public int? MaxUsersPerPass { get; set; } // Pentru acces de grup
        public int? MonthlyLimit { get; set; } // Pentru abonamente cu limită de utilizări pe lună
        public int? TotalUses { get; set; } // Pentru a numara toate Utilizarile
        public int? MaxUsersPerUsage { get; set; }

        public DateTime? ExpirationDateTime { get; set; } // Când expiră accesul

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }


        public ICollection<QrCode> QrCodes { get; set; }
    }
}
