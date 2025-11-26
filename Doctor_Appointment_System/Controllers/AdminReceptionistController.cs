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
    public class AdminReceptionistController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminReceptionistController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // LIST RECEPTIONISTS
        public async Task<IActionResult> Receptionists()
        {
            var receptionists = await _context.Receptionists
                .Include(r => r.User)
                .ToListAsync();

            return View(receptionists);
        }

        // =========================================================================
        // RECEPTIONIST MANAGEMENT
        // =========================================================================

        private static ReceptionistFormViewModel MapReceptionistToVm(Receptionist r)
        {
            var u = r.User;
            return new ReceptionistFormViewModel
            {
                Id = r.Id,
                UserId = r.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email!,
                PhoneNumber = u.PhoneNumber,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                PresentAddress = u.PresentAddress,
                DeskNo = r.DeskNo
            };
        }

        // CREATE RECEPTIONIST (GET)
        public IActionResult CreateReceptionist()
        {
            var vm = new ReceptionistFormViewModel();
            return View(vm);
        }

        // CREATE RECEPTIONIST (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReceptionist(ReceptionistFormViewModel model)
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

            if (!await _roleManager.RoleExistsAsync("Receptionist"))
            {
                var roleRes = await _roleManager.CreateAsync(new ApplicationRole { Name = "Receptionist" });
                if (!roleRes.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    ModelState.AddModelError(string.Empty, "Unable to create Receptionist role.");
                    return View(model);
                }
            }

            await _userManager.AddToRoleAsync(user, "Receptionist");

            var receptionist = new Receptionist
            {
                UserId = user.Id,
                DeskNo = model.DeskNo
            };

            _context.Receptionists.Add(receptionist);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Receptionist created successfully.";
            return RedirectToAction(nameof(Receptionists));
        }

        // EDIT RECEPTIONIST (GET)
        public async Task<IActionResult> EditReceptionist(int id)
        {
            var receptionist = await _context.Receptionists
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receptionist == null)
                return NotFound();

            var vm = MapReceptionistToVm(receptionist);
            return View(vm);
        }

        // EDIT RECEPTIONIST (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReceptionist(int id, ReceptionistFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var receptionist = await _context.Receptionists
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receptionist == null)
                return NotFound();

            var user = receptionist.User;

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

            receptionist.DeskNo = model.DeskNo;

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Receptionist updated successfully.";
            return RedirectToAction(nameof(Receptionists));
        }

        // DELETE RECEPTIONIST (GET)
        public async Task<IActionResult> DeleteReceptionist(int id)
        {
            var receptionist = await _context.Receptionists
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receptionist == null)
                return NotFound();

            return View(receptionist);
        }

        // DELETE RECEPTIONIST (POST)
        [HttpPost, ActionName("DeleteReceptionist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReceptionistConfirmed(int id)
        {
            var receptionist = await _context.Receptionists
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (receptionist == null)
                return NotFound();

            receptionist.User.IsActive = false;
            await _userManager.UpdateAsync(receptionist.User);

            _context.Receptionists.Remove(receptionist);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Receptionist deleted (user deactivated).";
            return RedirectToAction(nameof(Receptionists));
        }
    }
}
