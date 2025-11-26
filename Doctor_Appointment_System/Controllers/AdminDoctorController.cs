using System.Collections.Generic;
using System;
using System.Linq;
using System.Threading.Tasks;
using Doctor_Appointment_System.Data;
using Doctor_Appointment_System.Models;
using Doctor_Appointment_System.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Doctor_Appointment_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminDoctorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminDoctorController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // LIST DOCTORS
        public async Task<IActionResult> Doctors()
        {
            var doctors = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.DoctorSpecialties)
                    .ThenInclude(ds => ds.Specialty)
                .ToListAsync();

            return View(doctors);
        }

        // =========================================================================
        // DOCTOR MANAGEMENT
        // =========================================================================

        private async Task<IEnumerable<SelectListItem>> GetSpecialtySelectListAsync()
        {
            var specialties = await _context.Specialties
                .OrderBy(s => s.SpecialtyName)
                .ToListAsync();

            return specialties.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.SpecialtyName
            });
        }

        private async Task<DoctorFormViewModel> BuildDoctorFormViewModelAsync(Doctor? doctor = null)
        {
            var vm = new DoctorFormViewModel
            {
                SpecialtyOptions = await GetSpecialtySelectListAsync()
            };

            if (doctor != null)
            {
                var user = doctor.User;

                vm.Id = doctor.Id;
                vm.UserId = doctor.UserId;
                vm.FirstName = user.FirstName;
                vm.LastName = user.LastName;
                vm.Email = user.Email!;
                vm.PhoneNumber = user.PhoneNumber;
                vm.DateOfBirth = user.DateOfBirth;
                vm.Gender = user.Gender;
                vm.PresentAddress = user.PresentAddress;

                vm.LicenseNumber = doctor.LicenseNumber;
                vm.Qualification = doctor.Qualification;
                vm.Experience = doctor.Experience;
                vm.VisitCharge = doctor.VisitCharge;
                vm.Description = doctor.Description;
                vm.IsAvailable = doctor.IsAvailable;
                vm.RoomNo = doctor.RoomNo;
                vm.MaxAppointmentsPerDay = doctor.MaxAppointmentsPerDay;
                vm.AutoAcceptAppointments = doctor.AutoAcceptAppointments;

                vm.SelectedSpecialtyIds = doctor.DoctorSpecialties
                    .Select(ds => ds.SpecialtyId)
                    .ToList();
            }

            return vm;
        }

        // CREATE DOCTOR (GET)
        public async Task<IActionResult> CreateDoctor()
        {
            var vm = await BuildDoctorFormViewModelAsync();
            return View(vm);
        }

        // CREATE DOCTOR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDoctor(DoctorFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", "Password is required.");
            }

            if (!ModelState.IsValid)
            {
                model.SpecialtyOptions = await GetSpecialtySelectListAsync();
                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "A user with this email already exists.");
                model.SpecialtyOptions = await GetSpecialtySelectListAsync();
                return View(model);
            }

            var currentUserId = _userManager.GetUserId(User);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                PresentAddress = model.PresentAddress,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                RegisteredByUserId = currentUserId
            };

            var userResult = await _userManager.CreateAsync(user, model.Password!);
            if (!userResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty,
                    userResult.Errors.FirstOrDefault()?.Description ?? "Could not create user.");
                model.SpecialtyOptions = await GetSpecialtySelectListAsync();
                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync("Doctor"))
            {
                var roleRes = await _roleManager.CreateAsync(new ApplicationRole { Name = "Doctor" });
                if (!roleRes.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    ModelState.AddModelError(string.Empty, "Unable to create Doctor role.");
                    model.SpecialtyOptions = await GetSpecialtySelectListAsync();
                    return View(model);
                }
            }

            await _userManager.AddToRoleAsync(user, "Doctor");

            var doctor = new Doctor
            {
                UserId = user.Id,
                LicenseNumber = model.LicenseNumber,
                Qualification = model.Qualification,
                Experience = model.Experience,
                VisitCharge = model.VisitCharge,
                Description = model.Description,
                IsAvailable = model.IsAvailable,
                RoomNo = model.RoomNo,
                MaxAppointmentsPerDay = model.MaxAppointmentsPerDay,
                AutoAcceptAppointments = model.AutoAcceptAppointments
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            if (model.SelectedSpecialtyIds != null && model.SelectedSpecialtyIds.Any())
            {
                foreach (var specialtyId in model.SelectedSpecialtyIds)
                {
                    _context.DoctorSpecialties.Add(new DoctorSpecialty
                    {
                        DoctorId = doctor.Id,
                        SpecialtyId = specialtyId
                    });
                }

                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Doctor created successfully.";
            return RedirectToAction(nameof(Doctors));
        }

        // EDIT DOCTOR (GET)
        public async Task<IActionResult> EditDoctor(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.DoctorSpecialties)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return NotFound();

            var vm = await BuildDoctorFormViewModelAsync(doctor);
            return View(vm);
        }

        // EDIT DOCTOR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDoctor(int id, DoctorFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.SpecialtyOptions = await GetSpecialtySelectListAsync();
                return View(model);
            }

            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.DoctorSpecialties)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return NotFound();

            var user = doctor.User;

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;
            user.PresentAddress = model.PresentAddress;

            if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null && existingUser.Id != user.Id)
                {
                    ModelState.AddModelError("Email", "A user with this email already exists.");
                    model.SpecialtyOptions = await GetSpecialtySelectListAsync();
                    return View(model);
                }

                user.Email = model.Email;
                user.UserName = model.Email;
            }

            await _userManager.UpdateAsync(user);

            doctor.LicenseNumber = model.LicenseNumber;
            doctor.Qualification = model.Qualification;
            doctor.Experience = model.Experience;
            doctor.VisitCharge = model.VisitCharge;
            doctor.Description = model.Description;
            doctor.IsAvailable = model.IsAvailable;
            doctor.RoomNo = model.RoomNo;
            doctor.MaxAppointmentsPerDay = model.MaxAppointmentsPerDay;
            doctor.AutoAcceptAppointments = model.AutoAcceptAppointments;

            _context.DoctorSpecialties.RemoveRange(doctor.DoctorSpecialties);

            if (model.SelectedSpecialtyIds != null && model.SelectedSpecialtyIds.Any())
            {
                foreach (var specialtyId in model.SelectedSpecialtyIds)
                {
                    _context.DoctorSpecialties.Add(new DoctorSpecialty
                    {
                        DoctorId = doctor.Id,
                        SpecialtyId = specialtyId
                    });
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Doctor updated successfully.";
            return RedirectToAction(nameof(Doctors));
        }

        // DELETE DOCTOR (GET)
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return NotFound();

            return View(doctor);
        }

        // DELETE DOCTOR (POST)
        [HttpPost, ActionName("DeleteDoctor")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteDoctorConfirmed(int id)
        {
            var doctor = await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.DoctorSpecialties)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (doctor == null)
                return NotFound();

            _context.DoctorSpecialties.RemoveRange(doctor.DoctorSpecialties);

            doctor.User.IsActive = false;
            await _userManager.UpdateAsync(doctor.User);

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Doctor deleted (user deactivated).";
            return RedirectToAction(nameof(Doctors));
        }
    }
}
