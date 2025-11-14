using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    public class ApplicationRole : IdentityRole
    {
        [MaxLength(250)]
        public string? Description { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }
            = new List<ApplicationUserRole>();
    }
}
