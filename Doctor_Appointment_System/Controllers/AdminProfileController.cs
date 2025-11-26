using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Doctor_Appointment_System.Models;
using Doctor_Appointment_System.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Doctor_Appointment_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminProfileController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public AdminProfileController(
            UserManager<ApplicationUser> userManager,
            IWebHostEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }

        // =========================================================================
        // ADMIN - MY PROFILE (GET)
        // =========================================================================

        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var roleName = roles.FirstOrDefault() ?? "Admin";

            var virtualImagePath = $"/uploads/profile-images/{user.Id}.jpg";

            var vm = new AdminProfileViewModel
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                PhoneNumber = user.PhoneNumber,
                DateOfBirth = user.DateOfBirth,
                Gender = user.Gender,
                PresentAddress = user.PresentAddress,
                IsActive = user.IsActive,
                CreatedDate = user.CreatedDate,
                RoleName = roleName,
                ProfileImageUrl = virtualImagePath   // ✅ correct property name
            };

            return View(vm);
        }

        // =========================================================================
        // ADMIN - MY PROFILE (POST)
        // =========================================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MyProfile(AdminProfileViewModel model, IFormFile? ProfileImage)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return NotFound();

            if (!ModelState.IsValid)
            {
                var roles = await _userManager.GetRolesAsync(user);
                model.RoleName = roles.FirstOrDefault() ?? "Admin";
                model.IsActive = user.IsActive;
                model.CreatedDate = user.CreatedDate;
                model.ProfileImageUrl = $"/uploads/profile-images/{user.Id}.jpg";

                return View(model);
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;
            user.DateOfBirth = model.DateOfBirth;
            user.Gender = model.Gender;
            user.PresentAddress = model.PresentAddress;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                var roles = await _userManager.GetRolesAsync(user);
                model.RoleName = roles.FirstOrDefault() ?? "Admin";
                model.IsActive = user.IsActive;
                model.CreatedDate = user.CreatedDate;
                model.ProfileImageUrl = $"/uploads/profile-images/{user.Id}.jpg";

                return View(model);
            }

            if (ProfileImage != null && ProfileImage.Length > 0)
            {
                var uploadsRoot = Path.Combine(_env.WebRootPath, "uploads", "profile-images");
                Directory.CreateDirectory(uploadsRoot);

                var filePath = Path.Combine(uploadsRoot, user.Id + ".jpg");

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await ProfileImage.CopyToAsync(stream);
                }
            }

            TempData["SuccessMessage"] = "Profile updated successfully.";
            return RedirectToAction(nameof(MyProfile));
        }
    }
}
