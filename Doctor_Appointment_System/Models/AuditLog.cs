using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    public class AuditLog
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        /// <summary>
        /// Create / Update / Delete / Login etc.
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Action { get; set; } = null!;

        /// <summary>
        /// User / Appointment / Payment / Doctor etc.
        /// </summary>
        [Required]
        [StringLength(50)]
        public string EntityType { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string EntityId { get; set; } = null!;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        [StringLength(45)]
        public string? IPAddress { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
