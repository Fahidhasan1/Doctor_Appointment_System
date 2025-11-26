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
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // ---------------------------------------
        // MANAGE ADMIN USERS – LIST PAGE
        // ---------------------------------------
        public async Task<IActionResult> Admins()
        {
            var admins = await _context.Admins
                .Include(a => a.User)
                .ToListAsync();

            return View(admins);
        }

        // =========================================================================
        // ADMIN USER MANAGEMENT
        // =========================================================================

        private static AdminUserFormViewModel MapAdminToVm(Admin a)
        {
            var u = a.User;
            return new AdminUserFormViewModel
            {
                Id = a.Id,
                UserId = a.UserId,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email!,
                PhoneNumber = u.PhoneNumber,
                DateOfBirth = u.DateOfBirth,
                Gender = u.Gender,
                PresentAddress = u.PresentAddress
            };
        }

        public IActionResult CreateAdmin()
        {
            var vm = new AdminUserFormViewModel();
            return View(vm);
        }

        // CREATE ADMIN (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAdmin(AdminUserFormViewModel model)
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

            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                var roleRes = await _roleManager.CreateAsync(new ApplicationRole { Name = "Admin" });
                if (!roleRes.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    ModelState.AddModelError(string.Empty, "Unable to create Admin role.");
                    return View(model);
                }
            }

            await _userManager.AddToRoleAsync(user, "Admin");

            var admin = new Admin
            {
                UserId = user.Id
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Admin created successfully.";
            return RedirectToAction(nameof(Admins));
        }

        // EDIT ADMIN (GET)
        public async Task<IActionResult> EditAdmin(int id)
        {
            var admin = await _context.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (admin == null)
                return NotFound();

            var vm = MapAdminToVm(admin);
            return View(vm);
        }

        // EDIT ADMIN (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAdmin(int id, AdminUserFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var admin = await _context.Admins
                .Include(a => a.User)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (admin == null)
                return NotFound();

            var user = admin.User;

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;
            user.PresentAddress = model.PresentAddress;

            if (!string.Equals(user.Email, model.Email, StringComparison.OrdinalIgnoreCase))
            {
                user.Email = model.Email;
                user.UserName = model.Email;
            }

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Admin updated successfully.";
            return RedirectToAction(nameof(Admins));
        }

        //// DELETE ADMIN (GET)
        //public async Task<IActionResult> DeleteAdmin(int id)
        //{
        //    var admin = await _context.Admins
        //        .Include(a => a.User)
        //        .FirstOrDefaultAsync(a => a.Id == id);

        //    if (admin == null)
        //        return NotFound();

        //    return View(admin);
        //}

        //// DELETE ADMIN (POST)
        //[HttpPost, ActionName("DeleteAdmin")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteAdminConfirmed(int id)
        //{
        //    var admin = await _context.Admins
        //        .Include(a => a.User)
        //        .FirstOrDefaultAsync(a => a.Id == id);

        //    if (admin == null)
        //        return NotFound();

        //    var currentUserId = _userManager.GetUserId(User);
        //    if (admin.UserId == currentUserId)
        //    {
        //        TempData["ErrorMessage"] = "You cannot delete your own admin account.";
        //        return RedirectToAction(nameof(Admins));
        //    }

        //    admin.User.IsActive = false;
        //    await _userManager.UpdateAsync(admin.User);

        //    _context.Admins.Remove(admin);
        //    await _context.SaveChangesAsync();

        //    TempData["SuccessMessage"] = "Admin deleted (user deactivated).";
        //    return RedirectToAction(nameof(Admins));
        //}
    }
}
