using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyWebApi.Models
{
    public class Audio
    {
        [Key]
        public int Id { get; set; }

        public int PoiContentId { get; set; }

        [Required]
        [StringLength(500)]
        public string FileUrl { get; set; } = string.Empty;

        public int? DurationSeconds { get; set; }

        public int? FileSizeKb { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [JsonIgnore]
        [ForeignKey(nameof(PoiContentId))]
        public PoiContent PoiContent { get; set; } = null!;

        [JsonIgnore]
        public ICollection<ListenLog> ListenLogs { get; set; } = new List<ListenLog>();
    }
}
