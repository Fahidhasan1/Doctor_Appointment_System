//using Doctor_Appointment_System.Data;
//using Doctor_Appointment_System.Models;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//// -------------------------------------------------------
////   DATABASE CONNECTION → SQL Server (AppointmentDB)
////   Make sure appsettings.json has a "DefaultConnection"
////   pointing to your AppointmentDB in SSMS.
//// -------------------------------------------------------
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//// -------------------------------------------------------
////   IDENTITY CONFIGURATION (ApplicationUser + ApplicationRole)
//// -------------------------------------------------------
//builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
//{
//    // You can relax these for testing if you want
//    options.Password.RequireDigit = true;
//    options.Password.RequireLowercase = true;
//    options.Password.RequireUppercase = true;
//    options.Password.RequireNonAlphanumeric = false;
//    options.Password.RequiredLength = 6;

//    options.User.RequireUniqueEmail = true;
//})
//    .AddEntityFrameworkStores<ApplicationDbContext>()
//    .AddDefaultTokenProviders();

//// Configure application cookie (login / access denied paths)
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.LoginPath = "/Account/Login";           // You will create this controller/view
//    options.AccessDeniedPath = "/Account/AccessDenied";
//    options.LogoutPath = "/Account/Logout";
//});

//// -------------------------------------------------------
////   MVC + RAZOR PAGES
//// -------------------------------------------------------
//builder.Services.AddControllersWithViews();
//builder.Services.AddRazorPages();

//var app = builder.Build();

//// -------------------------------------------------------
////   MIDDLEWARE PIPELINE
//// -------------------------------------------------------
//if (app.Environment.IsDevelopment())
//{
//    app.UseDeveloperExceptionPage();
//}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();

//app.UseRouting();

//app.UseAuthentication();
//app.UseAuthorization();

//// -------------------------------------------------------
////   ROUTING
//// -------------------------------------------------------

//// Default MVC route: /{controller=Home}/{action=Index}/{id?}

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}");


//// Razor Pages (if you later scaffold Identity UI or use RP)
//app.MapRazorPages();

//// -------------------------------------------------------
////   SEED DEFAULT ROLES + INITIAL ADMIN
////   (Requirement: One Admin is registered initially)
//// -------------------------------------------------------
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    await SeedDataAsync(services);
//}

//app.Run();


//// =======================================================
////   LOCAL FUNCTION: Seed Roles and Default Admin User
//// =======================================================
//async Task SeedDataAsync(IServiceProvider services)
//{
//    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
//    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

//    string[] roleNames = { "Admin", "Doctor", "Receptionist", "Patient" };

//    // Create roles if they do not exist
//    foreach (var roleName in roleNames)
//    {
//        var roleExists = await roleManager.RoleExistsAsync(roleName);
//        if (!roleExists)
//        {
//            await roleManager.CreateAsync(new ApplicationRole
//            {
//                Name = roleName
//            });
//        }
//    }

//    // Create default Admin user if not exists
//    var adminEmail = "admin@hospital.com";
//    var adminUserName = "admin@hospital.com";
//    var adminPassword = "Admin@123"; // For demo/dev only. Change in production.

//    var adminUser = await userManager.FindByEmailAsync(adminEmail);
//    if (adminUser == null)
//    {
//        adminUser = new ApplicationUser
//        {
//            UserName = adminUserName,
//            Email = adminEmail,
//            FirstName = "System",
//            LastName = "Admin",
//            EmailConfirmed = true,
//            IsActive = true,
//            CreatedDate = DateTime.UtcNow
//        };

//        var createAdminResult = await userManager.CreateAsync(adminUser, adminPassword);
//        if (createAdminResult.Succeeded)
//        {
//            // Assign Admin role
//            await userManager.AddToRoleAsync(adminUser, "Admin");

//            // Also create Admin profile record if you want
//            var context = services.GetRequiredService<ApplicationDbContext>();

//            // Only add Admin profile if it doesn't exist
//            if (!context.Admins.Any(a => a.UserId == adminUser.Id))
//            {
//                context.Admins.Add(new Admin
//                {
//                    UserId = adminUser.Id,
//                    OfficeNumber = "A-1"
//                });

//                await context.SaveChangesAsync();
//            }
//        }
//        else
//        {
//            // Optional: log/handle errors from admin creation
//            // e.g. write to console during development
//            Console.WriteLine("Failed to create default admin user:");
//            foreach (var error in createAdminResult.Errors)
//            {
//                Console.WriteLine($" - {error.Code}: {error.Description}");
//            }
//        }
//    }
//}


using Doctor_Appointment_System.Data;
using Doctor_Appointment_System.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// -------------------------------------------------------
//   DATABASE CONNECTION → SQL Server (AppointmentDB)
// -------------------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// -------------------------------------------------------
//   IDENTITY CONFIGURATION (ApplicationUser + ApplicationRole)
// -------------------------------------------------------
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    // Password rules
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    // User rules
    options.User.RequireUniqueEmail = true;

})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// -------------------------------------------------------
//   COOKIE SETTINGS — IMPORTANT FOR POPUP LOGIN/REGISTER
// -------------------------------------------------------
builder.Services.ConfigureApplicationCookie(options =>
{
    // These paths will redirect to your modal instead of a page
    options.LoginPath = "/?auth=login";             // Opens login modal on Home
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LogoutPath = "/Account/Logout";

    // Security settings
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(24);
});

// -------------------------------------------------------
//   MVC + RAZOR PAGES
// -------------------------------------------------------
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// -------------------------------------------------------
//   MIDDLEWARE PIPELINE
// -------------------------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// -------------------------------------------------------
//   ROUTING
// -------------------------------------------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// -------------------------------------------------------
//   SEED DEFAULT ROLES + INITIAL ADMIN USER
// -------------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedDataAsync(services);
}

app.Run();


// =======================================================
//   LOCAL FUNCTION: Seed Roles and Default Admin
// =======================================================
async Task SeedDataAsync(IServiceProvider services)
{
    var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var context = services.GetRequiredService<ApplicationDbContext>();

    string[] roleNames = { "Admin", "Doctor", "Receptionist", "Patient" };

    // Create roles if missing
    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new ApplicationRole { Name = roleName });
        }
    }

    // Seed default Admin
    var adminEmail = "admin@hospital.com";
    var adminPassword = "Admin@123";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new ApplicationUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            FirstName = "System",
            LastName = "Admin",
            EmailConfirmed = true,
            IsActive = true,
            CreatedDate = DateTime.UtcNow
        };

        var createAdmin = await userManager.CreateAsync(adminUser, adminPassword);

        if (createAdmin.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");

            // Add admin profile if not exists
            if (!context.Admins.Any(a => a.UserId == adminUser.Id))
            {
                context.Admins.Add(new Admin
                {
                    UserId = adminUser.Id,
                    OfficeNumber = "A-1"
                });
                await context.SaveChangesAsync();
            }
        }
        else
        {
            Console.WriteLine("Admin creation failed:");
            foreach (var error in createAdmin.Errors)
                Console.WriteLine($" - {error.Description}");
        }
    }
}

