using System;
using System.ComponentModel.DataAnnotations;

namespace Doctor_Appointment_System.Models.ViewModels
{
    public class ReceptionistFormViewModel
    {
        public int? Id { get; set; }          // Receptionist.Id
        public string? UserId { get; set; }   // ApplicationUser.Id

        // ---------------------------
        // Identity user fields
        // ---------------------------
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
        public string Email { get; set; } = null!;

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

        // Password (create only)
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match.")]
        public string? ConfirmPassword { get; set; }

        // ---------------------------
        // Receptionist profile
        // ---------------------------
        [StringLength(20)]
        [Display(Name = "Desk no.")]
        public string? DeskNo { get; set; }
    }
}
