using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [StringLength(10)]
        public string? BloodGroup { get; set; }

        [StringLength(100)]
        public string? EmergencyContactName { get; set; }

        [StringLength(20)]
        public string? EmergencyContact { get; set; }

        // Navigation
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
