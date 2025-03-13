using Microsoft.EntityFrameworkCore;
using SmartPass.Repository.Models.EntityInterfaces;
using System.ComponentModel.DataAnnotations;

namespace SmartPass.Repository.Models.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class CardReader : IBaseEntity
    {
        [Required]
        [Key]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Name { get; set; } 

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public Guid ZoneId { get; set; }
        public Guid? OppositeCardReaderId { get; set; }

        public DateTime? LastUsingUtcDate { get; set; } 

        [Required]
        public DateTime CreateUtcDate { get; set; }
        public DateTime? UpdateUtcDate { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
        public DateTime? DeletedUtcDate { get; set; }


        public Zone Zone { get; set; }
        public CardReader OppositeCardReader { get; set; }
        public ICollection<AccessLog> Sessions { get; set; } 
    }
}
