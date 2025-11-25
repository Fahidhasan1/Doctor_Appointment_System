using System;
using System.ComponentModel.DataAnnotations;

namespace Doctor_Appointment_System.Models.ViewModels
{
    public class RegisterViewModel
    {
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
        [Display(Name = "Email address")]
        public string Email { get; set; } = null!;

        // Simpler validation: just required + max length (no [Phone])
        [Required]
        [StringLength(20, ErrorMessage = "Mobile number is too long.")]
        [Display(Name = "Mobile number")]
        public string PhoneNumber { get; set; } = null!;

        // Optional date – can be null if user doesn’t fill it
        [DataType(DataType.Date)]
        [Display(Name = "Date of birth")]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        // No DataAnnotation – we validate this manually in the controller
        [Display(Name = "I agree to the terms & conditions and privacy policy.")]
        public bool AcceptTerms { get; set; }
    }
}
