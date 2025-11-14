
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        public DateTime? DateOfBirth { get; set; }

        public Gender Gender { get; set; } = Gender.Unknown;

        [MaxLength(250)]
        public string? PresentAddress { get; set; }

        [MaxLength(250)]
        public string? PermanentAddress { get; set; }

        [MaxLength(20)]
        public string? NID { get; set; }

        [MaxLength(255)]
        public string? ProfilePicturePath { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? LastModifiedDate { get; set; }

        public DateTime? LastLoginDate { get; set; }

        
        public ICollection<ApplicationUserRole> UserRoles { get; set; }
            = new List<ApplicationUserRole>();

        
        public AdminProfile? AdminProfile { get; set; }

        public DoctorProfile? DoctorProfile { get; set; }

        public PatientProfile? PatientProfile { get; set; }

        public ReceptionistProfile? ReceptionistProfile { get; set; }
    }
}
