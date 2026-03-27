using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MyWebApi.Models
{
    public class ListenLog
    {
        [Key]
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int AudioId { get; set; }

        public DateTime ListenedAt { get; set; } = DateTime.UtcNow;

        public int? ListenDuration { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(AudioId))]
        public Audio Audio { get; set; } = null!;
    }
}
