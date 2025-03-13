using Microsoft.EntityFrameworkCore;
using SmartPass.Repository.Models.EntityInterfaces;
using System.ComponentModel.DataAnnotations;

namespace SmartPass.Repository.Models.Entities
{
    [Index(nameof(UserName), IsUnique = true)]
    public class User : IBaseEntity
    {
        [Required]
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string UserName { get; set; }

        [Required]
        [StringLength(44)]
        public string Password { get; set; }

        [MaxLength(50)]
        public string? Department { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public bool IsSynchronized { get; set; }

        public DateTime? LastSynchronizedUtcDate { get; set; }

        [Required]
        public DateTime CreateUtcDate { get; set; }
        public DateTime? UpdateUtcDate { get; set; }


        [Required]
        public bool IsDeleted { get; set; }
        public DateTime? DeletedUtcDate { get; set; }


        public ICollection<AccessCard> AccessCards { get; set; }
        public ICollection<UserRole> UserRoles { get; set; }
        public UserAuthData UserAuthData { get; set; }
    }
}
