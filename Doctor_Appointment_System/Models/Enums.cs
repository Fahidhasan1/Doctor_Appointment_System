using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Doctor_Appointment_System.Models
{
    // ---------------------------
    // Appointment Status Enum
    // ---------------------------
    public enum AppointmentStatus
    {
        Pending,
        Accepted,
        Completed,
        Cancelled
    }

    // ---------------------------
    // Payment Status Enum
    // ---------------------------
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    // ---------------------------
    // Payment Method Enum
    // ---------------------------
    public enum PaymentMethod
    {
        Bkash,
        Cash,
        Card
    }

    // ---------------------------
    // Notification Type Enum
    // ---------------------------
    public enum NotificationType
    {
        NewAppointment,
        Cancellation,
        Reminder,
        General
    }
}
