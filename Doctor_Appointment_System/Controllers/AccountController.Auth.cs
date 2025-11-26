using System;
using System.Linq;
using System.Threading.Tasks;
using Doctor_Appointment_System.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Doctor_Appointment_System.Controllers
{
    public partial class AccountController : Controller
    {
        // ============================
        // LOGIN (open login modal)
        // ============================
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            // Keep existing behavior: open Login modal via ?auth=login
            return RedirectToAction("Index", "Home", new { auth = "login" });
        }

        // ============================
        // LOGIN (POST)
        // ============================
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                var firstError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault()?.ErrorMessage;

                TempData["LoginError"] = firstError ?? "Please provide email and password.";
                return RedirectToAction("Index", "Home", new { auth = "login" });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["LoginError"] = "Invalid email or password.";
                return RedirectToAction("Index", "Home", new { auth = "login" });
            }

            if (!user.IsActive)
            {
                TempData["LoginError"] = "Your account is disabled. Please contact administrator.";
                return RedirectToAction("Index", "Home", new { auth = "login" });
            }

            var result = await _signInManager.PasswordSignInAsync(
                user,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                TempData["LoginError"] = "Invalid email or password.";
                return RedirectToAction("Index", "Home", new { auth = "login" });
            }

            // Get user roles
            var roles = await _userManager.GetRolesAsync(user);

            // Redirect based on role
            if (roles.Contains("Admin"))
                return RedirectToAction("Dashboard", "Admin");

            if (roles.Contains("Doctor"))
                return RedirectToAction("Dashboard", "Doctor");

            if (roles.Contains("Receptionist"))
                return RedirectToAction("Dashboard", "Receptionist");

            if (roles.Contains("Patient"))
                return RedirectToAction("Dashboard", "Patient");

            // Fallback: return to original URL if local
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // Final fallback
            return RedirectToAction("Index", "Home");
        }

        // ============================
        // LOGOUT
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ============================
        // ACCESS DENIED
        // ============================
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
