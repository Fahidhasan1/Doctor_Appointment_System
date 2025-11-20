using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;

namespace Doctor_Appointment_System.Models
{
    // Application user based on ASP.NET Core Identity
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(250)]
        public string? PresentAddress { get; set; }

        [StringLength(250)]
        public string? PermanentAddress { get; set; }

        [StringLength(50)]
        public string? NID { get; set; }

        [StringLength(250)]
        public string? ProfilePicturePath { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastModifiedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        /// <summary>
        /// Who registered this user (Admin or Receptionist). Null = self-registered patient.
        /// </summary>
        public string? RegisteredByUserId { get; set; }

        [ForeignKey(nameof(RegisteredByUserId))]
        public ApplicationUser? RegisteredByUser { get; set; }

        // Navigation
        public Admin? AdminProfile { get; set; }
        public Doctor? DoctorProfile { get; set; }
        public Receptionist? ReceptionistProfile { get; set; }
        public Patient? PatientProfile { get; set; }
    }
}
