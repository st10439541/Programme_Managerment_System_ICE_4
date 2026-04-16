using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammeManagementSystem.Data;
using ProgrammeManagementSystem.Models;

namespace ProgrammeManagementSystem.Controllers
{
    public class LecturersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public LecturersController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
            => View(await _context.Lecturers
                .Include(l => l.ModuleAssignments)
                .ThenInclude(a => a.Module)
                .OrderBy(l => l.LastName)
                .ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var lecturer = await _context.Lecturers
                .Include(l => l.ModuleAssignments)
                .ThenInclude(a => a.Module)
                .FirstOrDefaultAsync(l => l.LecturerID == id);

            return lecturer == null ? NotFound() : View(lecturer);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,Department")] Lecturer lecturer)
        {
            // Check for duplicate email
            var existingLecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.Email == lecturer.Email);

            if (existingLecturer != null)
            {
                ModelState.AddModelError("Email", "A lecturer with this email already exists.");
            }

            if (!ModelState.IsValid) return View(lecturer);

            _context.Add(lecturer);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Lecturer {lecturer.FirstName} {lecturer.LastName} added.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var lecturer = await _context.Lecturers.FindAsync(id);
            return lecturer == null ? NotFound() : View(lecturer);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LecturerID,FirstName,LastName,Email,Department")] Lecturer lecturer)
        {
            if (id != lecturer.LecturerID) return NotFound();

            // Check for duplicate email (excluding current lecturer)
            var existingLecturer = await _context.Lecturers
                .FirstOrDefaultAsync(l => l.Email == lecturer.Email && l.LecturerID != lecturer.LecturerID);

            if (existingLecturer != null)
            {
                ModelState.AddModelError("Email", "A lecturer with this email already exists.");
            }

            if (!ModelState.IsValid) return View(lecturer);

            _context.Update(lecturer);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Lecturer {lecturer.FirstName} {lecturer.LastName} updated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var lecturer = await _context.Lecturers
                .Include(l => l.ModuleAssignments)
                .FirstOrDefaultAsync(l => l.LecturerID == id);

            return lecturer == null ? NotFound() : View(lecturer);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lecturer = await _context.Lecturers
                .Include(l => l.ModuleAssignments)
                .FirstOrDefaultAsync(l => l.LecturerID == id);

            if (lecturer != null)
            {
                _context.Lecturers.Remove(lecturer);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Lecturer {lecturer.FirstName} {lecturer.LastName} deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}