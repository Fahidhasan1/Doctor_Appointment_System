using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Doctor_Appointment_System.Models
{
    public class ReceptionistProfile
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

  
        [MaxLength(50)]
        public string? DeskNumber { get; set; }

        [MaxLength(100)]
        public string? WorkShift { get; set; }    

        [MaxLength(500)]
        public string? Notes { get; set; }
    }
}
