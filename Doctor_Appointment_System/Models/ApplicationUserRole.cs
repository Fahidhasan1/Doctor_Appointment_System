using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    public class ApplicationUserRole : IdentityUserRole<string>
    {
        public ApplicationUser User { get; set; } = null!;
        public ApplicationRole Role { get; set; } = null!;
    }
}
