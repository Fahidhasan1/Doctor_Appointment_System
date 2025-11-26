using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Doctor_Appointment_System.Models.ViewModels
{
    public class DoctorFormViewModel
    {
        // Doctor entity
        public int? Id { get; set; }          // Doctor.Id
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

        // Password – required only when creating (we validate in controller)
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Password and confirm password do not match.")]
        public string? ConfirmPassword { get; set; }

        // ---------------------------
        // Doctor-specific fields
        // ---------------------------
        [Required]
        [StringLength(50)]
        [Display(Name = "License number")]
        public string LicenseNumber { get; set; } = null!;

        [Required]
        [StringLength(150)]
        [Display(Name = "Qualification")]
        public string Qualification { get; set; } = null!;

        [Range(0, 80)]
        [Display(Name = "Years of experience")]
        public int Experience { get; set; }

        [Range(0, 999999)]
        [Display(Name = "Visit charge")]
        public decimal VisitCharge { get; set; }

        [StringLength(1000)]
        [Display(Name = "Profile description")]
        public string? Description { get; set; }

        [Display(Name = "Available for appointments")]
        public bool IsAvailable { get; set; } = true;

        [StringLength(20)]
        [Display(Name = "Room no.")]
        public string? RoomNo { get; set; }

        [Range(0, 500)]
        [Display(Name = "Max appointments per day")]
        public int MaxAppointmentsPerDay { get; set; } = 0;

        [Display(Name = "Auto-accept appointments")]
        public bool AutoAcceptAppointments { get; set; } = true;

        // ---------------------------
        // Specialties
        // ---------------------------
        [Display(Name = "Specialties")]
        public List<int> SelectedSpecialtyIds { get; set; } = new();

        public IEnumerable<SelectListItem> SpecialtyOptions { get; set; }
            = new List<SelectListItem>();
        public IFormFile ImageDoctor { get; set; }
    }
}
