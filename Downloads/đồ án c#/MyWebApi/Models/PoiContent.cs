using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyWebApi.Models
{
    public class PoiContent
    {
        [Key]
        public int Id { get; set; }

        public int PoiId { get; set; }

        [Required]
        [StringLength(10)]
        public string LanguageCode { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = string.Empty;

        public string? ContentText { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(PoiId))]
        public Poi Poi { get; set; } = null!;

        public Audio? Audio { get; set; }
    }
}
