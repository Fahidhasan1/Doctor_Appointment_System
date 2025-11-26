using System;
using System.Linq;
using System.Threading.Tasks;
using Doctor_Appointment_System.Data;
using Doctor_Appointment_System.Models;
using Doctor_Appointment_System.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doctor_Appointment_System.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/[action]")]   // ✅ makes /Admin/Dashboard map here
    public class AdminDashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminDashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var today = DateTime.Today;
            var thisYear = today.Year;

            var totalAdmins = await _context.Admins.CountAsync();
            var totalDoctors = await _context.Doctors.CountAsync();
            var totalReceptionists = await _context.Receptionists.CountAsync();
            var totalPatients = await _context.Patients.CountAsync();
            var totalSpecialties = await _context.Specialties.CountAsync();
            var totalAppointments = await _context.Appointments.CountAsync();

            var todaysAppointmentsQuery = _context.Appointments
                .Include(a => a.Patient).ThenInclude(p => p.User)
                .Include(a => a.Doctor).ThenInclude(d => d.User)
                .Include(a => a.Doctor).ThenInclude(d => d.DoctorSpecialties)
                    .ThenInclude(ds => ds.Specialty)
                .Where(a => a.AppointmentDate.Date == today)
                .OrderBy(a => a.AppointmentTime);

            var todaysAppointments = await todaysAppointmentsQuery.ToListAsync();

            var appointmentRows = todaysAppointments.Select(a =>
            {
                var patientName = a.Patient != null
                    ? $"{a.Patient.User.FirstName} {a.Patient.User.LastName}"
                    : (a.GuestPatientName ?? "Guest Patient");

                var doctorName = a.Doctor != null
                    ? $"Dr. {a.Doctor.User.FirstName} {a.Doctor.User.LastName}"
                    : "Unknown Doctor";

                var specialtyName = a.Doctor?.DoctorSpecialties?
                    .Select(ds => ds.Specialty?.SpecialtyName)
                    .FirstOrDefault() ?? "General";

                return new AdminDashboardAppointmentRow
                {
                    AppointmentId = a.Id,
                    TimeText = DateTime.Today
                        .Add(a.AppointmentTime)          // AppointmentTime non-nullable
                        .ToString("hh:mm tt"),
                    PatientName = patientName,
                    DoctorName = doctorName,
                    SpecialtyName = specialtyName,
                    StatusText = a.Status.ToString()
                };
            }).ToList();

            var paymentsQuery = _context.Payments
                .Where(p =>
                    p.PaymentStatus == PaymentStatus.Completed &&
                    p.PaymentDate.Year == thisYear);

            var monthlyRevenue = await paymentsQuery
                .GroupBy(p => p.PaymentDate.Month)
                .Select(g => new MonthlyRevenuePoint
                {
                    Month = g.Key,
                    TotalAmount = g.Sum(x => x.Amount)
                })
                .ToListAsync();

            var monthlyRevenueTotal = monthlyRevenue.Sum(x => x.TotalAmount);

            var monthlyAppointments = await _context.Appointments
                .Where(a => a.AppointmentDate.Year == thisYear)
                .GroupBy(a => a.AppointmentDate.Month)
                .Select(g => new MonthlyAppointmentPoint
                {
                    Month = g.Key,
                    TotalAppointments = g.Count()
                })
                .ToListAsync();

            var model = new AdminDashboardViewModel
            {
                TotalAdmins = totalAdmins,
                TotalDoctors = totalDoctors,
                TotalReceptionists = totalReceptionists,
                TotalPatients = totalPatients,
                TotalSpecialties = totalSpecialties,
                TotalAppointments = totalAppointments,
                TodaysAppointmentCount = todaysAppointments.Count,
                MonthlyRevenue = monthlyRevenueTotal,
                MonthlyRevenueByMonth = monthlyRevenue,
                MonthlyAppointmentsByMonth = monthlyAppointments,
                TodaysAppointments = appointmentRows
            };

            return View(model);
        }
    }
}
