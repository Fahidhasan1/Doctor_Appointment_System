using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    public class Receptionist
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; } = null!;

        [StringLength(20)]
        public string? DeskNo { get; set; }

        // Navigation
        public ICollection<Appointment> CreatedAppointments { get; set; } = new List<Appointment>();
    }
}
