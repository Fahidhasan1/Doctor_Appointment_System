using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// Registered patient. Null if guest patient (quick booking).
        /// </summary>
        public int? PatientId { get; set; }

        [Required]
        public int DoctorId { get; set; }

        /// <summary>
        /// Receptionist who booked the appointment. Null if patient/admin booked online.
        /// </summary>
        public int? ReceptionistId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public Patient? Patient { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Doctor Doctor { get; set; } = null!;

        [ForeignKey(nameof(ReceptionistId))]
        public Receptionist? Receptionist { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan AppointmentTime { get; set; }

        // -----------------------------
        // ENUM — AppointmentStatus
        // -----------------------------
        [Required]
        [EnumDataType(typeof(AppointmentStatus))]
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        [Required]
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999)]
        public decimal VisitCharge { get; set; }

        public bool IsInstantBooking { get; set; } = false;

        public bool IsReminderSent { get; set; } = false;

        // Guest booking fields (instant booking)
        [StringLength(100)]
        public string? GuestPatientName { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? GuestPatientEmail { get; set; }

        [StringLength(20)]
        [Phone]
        public string? GuestPatientPhone { get; set; }

        // Navigation
        public Payment? Payment { get; set; }
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
