using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    public class Payment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AppointmentId { get; set; }

        [Required]
        public int PatientId { get; set; }

        // Admin or Receptionist who collected payment
        [Required]
        public string CollectedByUserId { get; set; } = null!;

        [ForeignKey(nameof(AppointmentId))]
        public Appointment Appointment { get; set; } = null!;

        [ForeignKey(nameof(PatientId))]
        public Patient Patient { get; set; } = null!;

        [ForeignKey(nameof(CollectedByUserId))]
        public ApplicationUser CollectedByUser { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 99999999)]
        public decimal Amount { get; set; }

        // -----------------------------
        // ENUM — PaymentMethod
        // -----------------------------
        [Required]
        [EnumDataType(typeof(PaymentMethod))]
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Bkash;

        [StringLength(100)]
        public string? TransactionId { get; set; }

        // -----------------------------
        // ENUM — PaymentStatus
        // -----------------------------
        [Required]
        [EnumDataType(typeof(PaymentStatus))]
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [StringLength(50)]
        public string? ReceiptNumber { get; set; }

        [StringLength(250)]
        public string? ReceiptPdfPath { get; set; }
    }
}
