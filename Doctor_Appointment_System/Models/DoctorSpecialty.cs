using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    public class DoctorSpecialty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int SpecialtyId { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public Doctor Doctor { get; set; } = null!;

        [ForeignKey(nameof(SpecialtyId))]
        public Specialty Specialty { get; set; } = null!;

        [Range(0, 80)]
        public int YearsOfExperience { get; set; }
    }
}
