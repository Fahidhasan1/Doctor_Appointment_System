using System;
using System.Collections.Generic;
using System.Linq;

namespace Doctor_Appointment_System.Models.ViewModels
{
    public class MonthlyRevenuePoint
    {
        public int Month { get; set; }          // 1–12
        public decimal TotalAmount { get; set; }

        public string MonthName =>
            new DateTime(2000, Month, 1).ToString("MMM");
    }

    public class MonthlyAppointmentPoint
    {
        public int Month { get; set; }          // 1–12
        public int TotalAppointments { get; set; }

        public string MonthName =>
            new DateTime(2000, Month, 1).ToString("MMM");
    }

    public class AdminDashboardAppointmentRow
    {
        public int AppointmentId { get; set; }
        public string TimeText { get; set; } = "";
        public string PatientName { get; set; } = "";
        public string DoctorName { get; set; } = "";
        public string SpecialtyName { get; set; } = "";
        public string StatusText { get; set; } = "";
    }

    public class AdminDashboardViewModel
    {
        // Top cards
        public int TotalAdmins { get; set; }
        public int TotalDoctors { get; set; }
        public int TotalReceptionists { get; set; }
        public int TotalPatients { get; set; }

        public int TotalSpecialties { get; set; }
        public int TotalAppointments { get; set; }
        public int TodaysAppointmentCount { get; set; }

        public decimal MonthlyRevenue { get; set; }

        // Charts
        public List<MonthlyRevenuePoint> MonthlyRevenueByMonth { get; set; } = new();
        public List<MonthlyAppointmentPoint> MonthlyAppointmentsByMonth { get; set; } = new();

        // Today’s appointments table
        public List<AdminDashboardAppointmentRow> TodaysAppointments { get; set; } = new();
    }
}
