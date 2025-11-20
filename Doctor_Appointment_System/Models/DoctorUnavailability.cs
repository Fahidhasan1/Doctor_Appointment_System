using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    public class DoctorUnavailability
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Doctor Doctor { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime UnavailableDate { get; set; }

        [StringLength(250)]
        public string? Reason { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
