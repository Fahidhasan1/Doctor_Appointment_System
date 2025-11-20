using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LicenseNumber { get; set; } = null!;

        [Required]
        [StringLength(150)]
        public string Qualification { get; set; } = null!;

        [Range(0, 80)]
        public int Experience { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999)]
        public decimal VisitCharge { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public bool IsAvailable { get; set; } = true;

        [Range(0, 5)]
        public double? Rating { get; set; }

        [StringLength(20)]
        public string? RoomNo { get; set; }

        [Range(0, 500)]
        public int MaxAppointmentsPerDay { get; set; } = 0;

        public bool AutoAcceptAppointments { get; set; } = true;

        // Navigation collections
        public ICollection<DoctorSpecialty> DoctorSpecialties { get; set; } = new List<DoctorSpecialty>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<DoctorSchedule> Schedules { get; set; } = new List<DoctorSchedule>();
        public ICollection<DoctorUnavailability> Unavailabilities { get; set; } = new List<DoctorUnavailability>();

        // 🔥 THIS WAS MISSING → Add this
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
