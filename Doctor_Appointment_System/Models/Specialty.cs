using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    public class Specialty
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string SpecialtyName { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        // Navigation
        public ICollection<DoctorSpecialty> DoctorSpecialties { get; set; } = new List<DoctorSpecialty>();
    }
}
