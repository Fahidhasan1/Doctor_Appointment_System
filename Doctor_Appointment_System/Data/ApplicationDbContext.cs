using System;
using Doctor_Appointment_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Doctor_Appointment_System.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets for your domain entities
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Receptionist> Receptionists { get; set; }

        public DbSet<Specialty> Specialties { get; set; }
        public DbSet<DoctorSpecialty> DoctorSpecialties { get; set; }

        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<DoctorSchedule> DoctorSchedules { get; set; }
        public DbSet<DoctorUnavailability> DoctorUnavailabilities { get; set; }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Review> Reviews { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // ------------------------------------
            // ENUM CONVERSIONS (store as string)
            // ------------------------------------

            builder.Entity<Appointment>()
                .Property(a => a.Status)
                .HasConversion<string>();

            builder.Entity<Payment>()
                .Property(p => p.PaymentStatus)
                .HasConversion<string>();

            builder.Entity<Payment>()
                .Property(p => p.PaymentMethod)
                .HasConversion<string>();

            builder.Entity<Notification>()
                .Property(n => n.NotificationType)
                .HasConversion<string>();

            // ------------------------------------
            // ONE-TO-ONE: ApplicationUser <-> Profiles
            // ------------------------------------

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.AdminProfile)
                .WithOne(a => a.User)
                .HasForeignKey<Admin>(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.DoctorProfile)
                .WithOne(d => d.User)
                .HasForeignKey<Doctor>(d => d.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.PatientProfile)
                .WithOne(p => p.User)
                .HasForeignKey<Patient>(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ApplicationUser>()
                .HasOne(u => u.ReceptionistProfile)
                .WithOne(r => r.User)
                .HasForeignKey<Receptionist>(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User who registered another user (RegisteredByUser)
            builder.Entity<ApplicationUser>()
                .HasOne(u => u.RegisteredByUser)
                .WithMany()
                .HasForeignKey(u => u.RegisteredByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------
            // DOCTOR RELATIONSHIPS
            // ------------------------------------

            // Doctor 1 - many DoctorSchedule
            builder.Entity<DoctorSchedule>()
                .HasOne(ds => ds.Doctor)
                .WithMany(d => d.Schedules)
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Doctor 1 - many DoctorUnavailability
            builder.Entity<DoctorUnavailability>()
                .HasOne(du => du.Doctor)
                .WithMany(d => d.Unavailabilities)
                .HasForeignKey(du => du.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Doctor <-> DoctorSpecialty (many to many via join table)
            builder.Entity<DoctorSpecialty>()
                .HasOne(ds => ds.Doctor)
                .WithMany(d => d.DoctorSpecialties)
                .HasForeignKey(ds => ds.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<DoctorSpecialty>()
                .HasOne(ds => ds.Specialty)
                .WithMany(s => s.DoctorSpecialties)
                .HasForeignKey(ds => ds.SpecialtyId)
                .OnDelete(DeleteBehavior.Cascade);

            // ------------------------------------
            // APPOINTMENT RELATIONSHIPS
            // ------------------------------------

            // Doctor 1 - many Appointments
            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Patient 1 - many Appointments (optional)
            builder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Receptionist 1 - many CreatedAppointments (optional)
            builder.Entity<Appointment>()
                .HasOne(a => a.Receptionist)
                .WithMany(r => r.CreatedAppointments)
                .HasForeignKey(a => a.ReceptionistId)
                .OnDelete(DeleteBehavior.Restrict);

            // Appointment 1 - 1 Payment
            builder.Entity<Appointment>()
                .HasOne(a => a.Payment)
                .WithOne(p => p.Appointment)
                .HasForeignKey<Payment>(p => p.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Appointment 1 - many Notifications
            builder.Entity<Notification>()
                .HasOne(n => n.Appointment)
                .WithMany(a => a.Notifications)
                .HasForeignKey(n => n.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Appointment 1 - many Reviews
            builder.Entity<Review>()
                .HasOne(r => r.Appointment)
                .WithMany(a => a.Reviews)
                .HasForeignKey(r => r.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------
            // PAYMENT RELATIONSHIPS
            // ------------------------------------

            builder.Entity<Payment>()
                .HasOne(p => p.Patient)
                .WithMany(pa => pa.Payments)
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Payment>()
                .HasOne(p => p.CollectedByUser)
                .WithMany()
                .HasForeignKey(p => p.CollectedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------
            // NOTIFICATION RELATIONSHIPS
            // ------------------------------------

            builder.Entity<Notification>()
                .HasOne(n => n.User)
                .WithMany()
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ------------------------------------
            // REVIEW RELATIONSHIPS
            // ------------------------------------

            builder.Entity<Review>()
                .HasOne(r => r.Patient)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Review>()
                .HasOne(r => r.Doctor)
                .WithMany(d => d.Reviews)
                .HasForeignKey(r => r.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------
            // AUDIT LOG
            // ------------------------------------

            builder.Entity<AuditLog>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // ------------------------------------
            // PASSWORD RESET TOKEN
            // ------------------------------------

            builder.Entity<PasswordResetToken>()
                .HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // ------------------------------------
            // OPTIONAL: TABLE NAMES (if you want explicit names)
            // ------------------------------------
            // builder.Entity<Admin>().ToTable("Admins");
            // builder.Entity<Doctor>().ToTable("Doctors");
            // builder.Entity<Patient>().ToTable("Patients");
            // builder.Entity<Receptionist>().ToTable("Receptionists");
            // builder.Entity<Specialty>().ToTable("Specialties");
            // builder.Entity<DoctorSpecialty>().ToTable("DoctorSpecialties");
            // builder.Entity<Appointment>().ToTable("Appointments");
            // builder.Entity<DoctorSchedule>().ToTable("DoctorSchedules");
            // builder.Entity<DoctorUnavailability>().ToTable("DoctorUnavailabilities");
            // builder.Entity<Payment>().ToTable("Payments");
            // builder.Entity<Notification>().ToTable("Notifications");
            // builder.Entity<Review>().ToTable("Reviews");
            // builder.Entity<AuditLog>().ToTable("AuditLogs");
            // builder.Entity<PasswordResetToken>().ToTable("PasswordResetTokens");
        }
    }
}
