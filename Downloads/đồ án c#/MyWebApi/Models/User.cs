using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MyWebApi.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty; // "Admin", "Owner", "AppUser"

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        public ICollection<Poi> OwnedPois { get; set; } = new List<Poi>();
        
        [JsonIgnore]
        public ICollection<ListenLog> ListenLogs { get; set; } = new List<ListenLog>();
    }
}
