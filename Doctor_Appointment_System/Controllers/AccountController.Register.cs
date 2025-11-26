using System;
using System.Linq;
using System.Threading.Tasks;
using Doctor_Appointment_System.Data;
using Doctor_Appointment_System.Models;
using Doctor_Appointment_System.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Doctor_Appointment_System.Controllers
{
    public partial class AccountController : Controller
    {
        // ============================
        // REGISTER (open register modal)
        // ============================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            // Keep your existing behavior:
            // open the Register modal on the Home page via ?auth=register
            return RedirectToAction("Index", "Home", new { auth = "register" });
        }

        // ============================
        // REGISTER (POST)
        // ============================
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // 1) Validate model
            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage;

                TempData["RegisterError"] =
                    firstError ?? "Please fill all required fields correctly.";

                return RedirectToAction("Index", "Home", new { auth = "register" });
            }

            if (!model.AcceptTerms)
            {
                TempData["RegisterError"] = "You must agree to the terms and conditions.";
                return RedirectToAction("Index", "Home", new { auth = "register" });
            }

            // 2) Check email uniqueness
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                TempData["RegisterError"] = "An account with this email already exists.";
                return RedirectToAction("Index", "Home", new { auth = "register" });
            }

            // 3) Create Identity user
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                var identityError = result.Errors.FirstOrDefault()?.Description
                                    ?? "Unable to create account.";
                TempData["RegisterError"] = identityError;
                return RedirectToAction("Index", "Home", new { auth = "register" });
            }

            // 4) Ensure Patient role exists
            if (!await _roleManager.RoleExistsAsync("Patient"))
            {
                var roleResult = await _roleManager.CreateAsync(new ApplicationRole { Name = "Patient" });
                if (!roleResult.Succeeded)
                {
                    // Clean up created user if role creation fails
                    await _userManager.DeleteAsync(user);

                    TempData["RegisterError"] =
                        "Unable to configure patient role. Please contact administrator.";
                    return RedirectToAction("Index", "Home", new { auth = "register" });
                }
            }

            // 5) Add user to Patient role
            await _userManager.AddToRoleAsync(user, "Patient");

            // 6) Create Patient profile (this is where DB save happens)
            //    IMPORTANT: this ensures registration is stored as a Patient in your DB.
            var patientProfile = new Patient
            {
                UserId = user.Id
                // if you want to map any initial patient fields from RegisterViewModel later,
                // you can add them here
            };

            _context.Patients.Add(patientProfile);
            await _context.SaveChangesAsync();

            // 7) Show success message & redirect to LOGIN modal
            TempData["RegisterSuccess"] = "Account created successfully. Please login to continue.";
            return RedirectToAction("Index", "Home", new { auth = "login" });
        }
    }
}
