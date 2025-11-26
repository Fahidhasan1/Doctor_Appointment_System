using System;
using System.Linq;
using System.Threading.Tasks;
using Doctor_Appointment_System.Data;
using Doctor_Appointment_System.Models;
using Doctor_Appointment_System.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doctor_Appointment_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPatientController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminPatientController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // LIST PATIENTS
        public async Task<IActionResult> Patients()
        {
            var patients = await _context.Patients
                .Include(p => p.User)
                .ToListAsync();

            return View(patients);
        }

        // =========================================================================
        // PATIENT MANAGEMENT
        // =========================================================================

        private static PatientFormViewModel MapPatientToVm(Patient p)
        {
            var u = p.User;
            return new PatientFormViewModel
            {
                Id = p.Id,
                UserId = p.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email!,
                PhoneNumber = u.PhoneNumber,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                PresentAddress = u.PresentAddress
            };
        }

        // CREATE PATIENT (GET)
        public IActionResult CreatePatient()
        {
            var vm = new PatientFormViewModel();
            return View(vm);
        }

        // CREATE PATIENT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePatient(PatientFormViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Password))
            {
                ModelState.AddModelError("Password", "Password is required.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "A user with this email already exists.");
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
                return View(model);
            }

            if (!await _roleManager.RoleExistsAsync("Patient"))
            {
                var roleRes = await _roleManager.CreateAsync(new ApplicationRole { Name = "Patient" });
                if (!roleRes.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    ModelState.AddModelError(string.Empty, "Unable to create Patient role.");
                    return View(model);
                }
            }

            await _userManager.AddToRoleAsync(user, "Patient");

            var patient = new Patient
            {
                UserId = user.Id
            };

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient created successfully.";
            return RedirectToAction(nameof(Patients));
        }

        // EDIT PATIENT (GET)
        public async Task<IActionResult> EditPatient(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
                return NotFound();

            var vm = MapPatientToVm(patient);
            return View(vm);
        }

        // EDIT PATIENT (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPatient(int id, PatientFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
                return NotFound();

            var user = patient.User;

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
                    return View(model);
                }

                user.Email = model.Email;
                user.UserName = model.Email;
            }

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient updated successfully.";
            return RedirectToAction(nameof(Patients));
        }

        // DELETE PATIENT (GET)
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
                return NotFound();

            return View(patient);
        }

        // DELETE PATIENT (POST)
        [HttpPost, ActionName("DeletePatient")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePatientConfirmed(int id)
        {
            var patient = await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (patient == null)
                return NotFound();

            patient.User.IsActive = false;
            await _userManager.UpdateAsync(patient.User);

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Patient deleted (user deactivated).";
            return RedirectToAction(nameof(Patients));
        }
    }
}
