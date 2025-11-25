using System.ComponentModel.DataAnnotations;

namespace Doctor_Appointment_System.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; } = null!;

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }
}
