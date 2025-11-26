using System;
using System.ComponentModel.DataAnnotations;

namespace Doctor_Appointment_System.Models.ViewModels
{
    public class AdminProfileViewModel
    {
        public string UserId { get; set; } = null!;

        [Display(Name = "Full name")]
        public string FullName => $"{FirstName} {LastName}";

        [Required]
        [StringLength(50)]
        [Display(Name = "First name")]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last name")]
        public string LastName { get; set; } = null!;

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;   // read-only in UI

        [Phone]
        [Display(Name = "Phone number")]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of birth")]
        public DateTime? DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(250)]
        [Display(Name = "Present address")]
        public string? PresentAddress { get; set; }

        // ---- Display-only extra info ----
        [Display(Name = "Active status")]
        public bool IsActive { get; set; }

        [Display(Name = "Registered on")]
        public DateTime CreatedDate { get; set; }

        [Display(Name = "Role")]
        public string RoleName { get; set; } = "Admin";

        // Profile image URL for <img>
        public string ProfileImageUrl { get; set; } = "";
    }
}
