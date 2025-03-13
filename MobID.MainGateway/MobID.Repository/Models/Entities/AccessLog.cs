using SmartPass.Repository.Models.EntityInterfaces;
using SmartPass.Repository.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace SmartPass.Repository.Models.Entities
{
    public class AccessLog : IBaseEntity
    {
        [Required]
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid AccessCardId { get; set; }

        [Required]
        public Guid CardReaderId { get; set; }

        [Required]
        public SessionStatus SessionStatus { get; set; }

        public string? Description { get; set; }

        public DateTime? StartUtcDate { get; set; }
        public DateTime? EndUtcDate { get; set; }

        [Required]
        public DateTime CreateUtcDate { get; set; }
        public DateTime? UpdateUtcDate { get; set; }

        [Required]
        public bool IsDeleted { get; set; }
        public DateTime? DeletedUtcDate { get; set; }

        public CardReader CardReader { get; set; }
        public AccessCard AccessCard { get; set; }
    }
}
