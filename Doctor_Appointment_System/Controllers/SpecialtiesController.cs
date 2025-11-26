using System.Linq;
using System.Threading.Tasks;
using Doctor_Appointment_System.Data;
using Doctor_Appointment_System.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Doctor_Appointment_System.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SpecialtiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SpecialtiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Specialties
        public async Task<IActionResult> Index()
        {
            var specialties = await _context.Specialties
                .OrderBy(s => s.SpecialtyName)
                .ToListAsync();

            return View(specialties);
        }

        // GET: /Specialties/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Specialties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Specialty model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // simple duplicate check
            bool exists = await _context.Specialties
                .AnyAsync(s => s.SpecialtyName == model.SpecialtyName);

            if (exists)
            {
                ModelState.AddModelError("SpecialtyName", "This specialty already exists.");
                return View(model);
            }

            _context.Specialties.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Specialty created successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Specialties/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        // POST: /Specialties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Specialty model)
        {
            if (id != model.Id)   // assumes primary key is "Id"
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }

            specialty.SpecialtyName = model.SpecialtyName;
            // if your Specialty has other fields (e.g. Description) update them here

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Specialty updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        // GET: /Specialties/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }

            return View(specialty);
        }

        // POST: /Specialties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var specialty = await _context.Specialties.FindAsync(id);
            if (specialty == null)
            {
                return NotFound();
            }

            // prevent deleting if any doctor is using this specialty
            bool inUse = await _context.DoctorSpecialties
                .AnyAsync(ds => ds.SpecialtyId == id);

            if (inUse)
            {
                TempData["ErrorMessage"] =
                    "Cannot delete this specialty because it is assigned to one or more doctors.";
                return RedirectToAction(nameof(Index));
            }

            _context.Specialties.Remove(specialty);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Specialty deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}
