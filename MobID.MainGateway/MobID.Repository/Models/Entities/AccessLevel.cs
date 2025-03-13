﻿using Microsoft.EntityFrameworkCore;
using SmartPass.Repository.Models.EntityInterfaces;
using System.ComponentModel.DataAnnotations;

namespace SmartPass.Repository.Models.Entities
{
    [Index(nameof(Name), IsUnique = true)]
    public class AccessLevel : IBaseEntity
    {
        [Required]
        [Key]
        public Guid Id {  get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public bool IsForSpecificZone { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }



        [Required]
        public DateTime CreateUtcDate { get; set; }
        public DateTime? UpdateUtcDate { get; set; }


        [Required]
        public bool IsDeleted { get; set; }
        public DateTime? DeletedUtcDate { get; set; }


        public ICollection<AccessCard> AccessCards { get; set; }
        public ICollection<Zone> Zones { get; set; }
    }
}
