using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    public class DoctorSchedule
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Doctor Doctor { get; set; } = null!;

        /// <summary>
        /// DayOfWeek enum (0 = Sunday, 1 = Monday, etc.).
        /// </summary>
        [Required]
        public DayOfWeek DayOfWeek { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        /// <summary>
        /// Duration of one slot in minutes.
        /// </summary>
        [Range(5, 240)]
        public int SlotDuration { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
