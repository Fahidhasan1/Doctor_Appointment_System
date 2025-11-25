//using System;
//using System.Linq;
//using System.Threading.Tasks;
//using Doctor_Appointment_System.Data;
//using Doctor_Appointment_System.Models;
//using Doctor_Appointment_System.Models.ViewModels;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;

//namespace Doctor_Appointment_System.Controllers
//{
//    public class AccountController : Controller
//    {
//        private readonly UserManager<ApplicationUser> _userManager;
//        private readonly SignInManager<ApplicationUser> _signInManager;
//        private readonly RoleManager<ApplicationRole> _roleManager;
//        private readonly ApplicationDbContext _context;

//        public AccountController(
//            UserManager<ApplicationUser> userManager,
//            SignInManager<ApplicationUser> signInManager,
//            RoleManager<ApplicationRole> roleManager,
//            ApplicationDbContext context)
//        {
//            _userManager = userManager;
//            _signInManager = signInManager;
//            _roleManager = roleManager;
//            _context = context;
//        }

//        // Always redirect GET login/register to Home with proper modal open
//        [HttpGet]
//        [AllowAnonymous]
//        public IActionResult Login(string? returnUrl = null)
//        {
//            return RedirectToAction("Index", "Home", new { auth = "login" });
//        }

//        [HttpGet]
//        [AllowAnonymous]
//        public IActionResult Register()
//        {
//            return RedirectToAction("Index", "Home", new { auth = "register" });
//        }

//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Register(RegisterViewModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                TempData["RegisterError"] = "Please fill all required fields correctly.";
//                return RedirectToAction("Index", "Home", new { auth = "register" });
//            }

//            var existingUser = await _userManager.FindByEmailAsync(model.Email);
//            if (existingUser != null)
//            {
//                TempData["RegisterError"] = "An account with this email already exists.";
//                return RedirectToAction("Index", "Home", new { auth = "register" });
//            }

//            var user = new ApplicationUser
//            {
//                UserName = model.Email,
//                Email = model.Email,
//                FirstName = model.FirstName,
//                LastName = model.LastName,
//                PhoneNumber = model.PhoneNumber,
//                DateOfBirth = model.DateOfBirth,
//                Gender = model.Gender,
//                IsActive = true
//            };

//            var result = await _userManager.CreateAsync(user, model.Password);
//            if (!result.Succeeded)
//            {
//                TempData["RegisterError"] = result.Errors.FirstOrDefault()?.Description ?? "Unable to create account.";
//                return RedirectToAction("Index", "Home", new { auth = "register" });
//            }

//            // Ensure Patient role
//            if (!await _roleManager.RoleExistsAsync("Patient"))
//            {
//                await _roleManager.CreateAsync(new ApplicationRole { Name = "Patient" });
//            }

//            await _userManager.AddToRoleAsync(user, "Patient");

//            // Create Patient profile
//            var patientProfile = new Patient
//            {
//                UserId = user.Id
//            };
//            _context.Patients.Add(patientProfile);
//            await _context.SaveChangesAsync();

//            await _signInManager.SignInAsync(user, isPersistent: false);
//            return RedirectToAction("Dashboard", "Patient");
//        }

//        [HttpPost]
//        [AllowAnonymous]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
//        {
//            if (!ModelState.IsValid)
//            {
//                TempData["LoginError"] = "Please provide email and password.";
//                return RedirectToAction("Index", "Home", new { auth = "login" });
//            }

//            var user = await _userManager.FindByEmailAsync(model.Email);
//            if (user == null)
//            {
//                TempData["LoginError"] = "Invalid email or password.";
//                return RedirectToAction("Index", "Home", new { auth = "login" });
//            }

//            if (!user.IsActive)
//            {
//                TempData["LoginError"] = "Your account is disabled. Please contact administrator.";
//                return RedirectToAction("Index", "Home", new { auth = "login" });
//            }

//            var result = await _signInManager.PasswordSignInAsync(
//                user,
//                model.Password,
//                model.RememberMe,
//                lockoutOnFailure: false);

//            if (!result.Succeeded)
//            {
//                TempData["LoginError"] = "Invalid email or password.";
//                return RedirectToAction("Index", "Home", new { auth = "login" });
//            }

//            var roles = await _userManager.GetRolesAsync(user);
//            if (roles.Contains("Admin"))
//                return RedirectToAction("Dashboard", "Admin");

//            if (roles.Contains("Doctor"))
//                return RedirectToAction("Dashboard", "Doctor");

//            if (roles.Contains("Receptionist"))
//                return RedirectToAction("Dashboard", "Receptionist");

//            if (roles.Contains("Patient"))
//                return RedirectToAction("Dashboard", "Patient");

//            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
//                return Redirect(returnUrl);

//            return RedirectToAction("Index", "Home");
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> Logout()
//        {
//            await _signInManager.SignOutAsync();
//            return RedirectToAction("Index", "Home");
//        }

//        [AllowAnonymous]
//        public IActionResult AccessDenied()
//        {
//            return View();
//        }
//    }
//}


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
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET -> open login modal
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            return RedirectToAction("Index", "Home", new { auth = "login" });
        }

        // GET -> open register modal
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return RedirectToAction("Index", "Home", new { auth = "register" });
        }

        // ========== REGISTER ==========
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // 1) Validate model
            if (!ModelState.IsValid)
            {
                // Get first error message to show instead of generic text
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

            // 3) Create identity user
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

            // 4) Ensure Patient role
            if (!await _roleManager.RoleExistsAsync("Patient"))
            {
                var roleResult = await _roleManager.CreateAsync(new ApplicationRole { Name = "Patient" });
                if (!roleResult.Succeeded)
                {
                    TempData["RegisterError"] = "Unable to configure patient role. Please contact administrator.";
                    await _userManager.DeleteAsync(user); // clean up
                    return RedirectToAction("Index", "Home", new { auth = "register" });
                }
            }

            // 5) Add to Patient role
            await _userManager.AddToRoleAsync(user, "Patient");

            //// 6) Create Patient profile
            //var patientProfile = new Patient
            //{
            //    UserId = user.Id
            //};
            //_context.Patients.Add(patientProfile);
            //await _context.SaveChangesAsync();

            //// 7) Auto login & redirect
            //await _signInManager.SignInAsync(user, isPersistent: false);
            //return RedirectToAction("Dashboard", "Patient");

            // 6) Create Patient profile...........................
            var patientProfile = new Patient
            {
                UserId = user.Id
            };
            _context.Patients.Add(patientProfile);
            await _context.SaveChangesAsync();

            // ❌ Remove auto login
            // await _signInManager.SignInAsync(user, isPersistent: false);
            // return RedirectToAction("Dashboard", "Patient");

            // ✅ Replace with this:
            TempData["LoginError"] = "Account created successfully. Please login to continue.";
            return RedirectToAction("Index", "Home", new { auth = "login" });

        }

        // ========== LOGIN ==========
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

            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
                return RedirectToAction("Dashboard", "Admin");
            if (roles.Contains("Doctor"))
                return RedirectToAction("Dashboard", "Doctor");
            if (roles.Contains("Receptionist"))
                return RedirectToAction("Dashboard", "Receptionist");
            if (roles.Contains("Patient"))
                return RedirectToAction("Dashboard", "Patient");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }

        // ========== LOGOUT ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
