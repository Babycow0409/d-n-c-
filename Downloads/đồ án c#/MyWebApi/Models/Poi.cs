using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyWebApi.Models
{
    public class Poi
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string InternalName { get; set; } = string.Empty;

        [Column(TypeName = "decimal(9, 6)")]
        public decimal Latitude { get; set; }

        [Column(TypeName = "decimal(9, 6)")]
        public decimal Longitude { get; set; }

        public int Radius { get; set; }

        public int Priority { get; set; } = 0;

        public int? OwnerId { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(OwnerId))]
        public User? Owner { get; set; }

        public ICollection<PoiContent> Contents { get; set; } = new List<PoiContent>();
    }
}
