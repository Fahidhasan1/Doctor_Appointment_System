using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_Appointment_System.Models
{
    public class AdminProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [MaxLength(100)]
        public string? Designation { get; set; }      

        [MaxLength(20)]
        public string? OfficePhone { get; set; }

        [MaxLength(150)]
        public string? OfficeLocation { get; set; }  

        public bool IsSuperAdmin { get; set; } = false;

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
