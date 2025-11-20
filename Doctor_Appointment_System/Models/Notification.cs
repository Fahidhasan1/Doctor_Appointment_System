using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        public int? AppointmentId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [ForeignKey(nameof(AppointmentId))]
        public Appointment? Appointment { get; set; }

        // -----------------------------
        // ENUM — NotificationType
        // -----------------------------
        [Required]
        [EnumDataType(typeof(NotificationType))]
        public NotificationType NotificationType { get; set; } = NotificationType.General;

        [Required]
        [StringLength(150)]
        public string Title { get; set; } = null!;

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = null!;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Email / SMS / InApp
        [StringLength(20)]
        public string? SentVia { get; set; }
    }
}
