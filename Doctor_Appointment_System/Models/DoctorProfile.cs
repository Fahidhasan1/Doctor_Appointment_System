using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_Appointment_System.Models
{
    public class DoctorProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [Required, MaxLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? Qualification { get; set; }    

        public int ExperienceYears { get; set; }

        [Range(0, 999999)]
        public decimal VisitCharge { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }     

        public bool IsAvailable { get; set; } = true;

       
        [Range(0, 5)]
        public double Rating { get; set; }

       
        [MaxLength(100)]
        public string? Specialization { get; set; }   

        [MaxLength(50)]
        public string? ClinicRoomNumber { get; set; }
    }
}
