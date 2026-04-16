using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProgrammeManagementSystem.Data;
using ProgrammeManagementSystem.Models;

namespace ProgrammeManagementSystem.Controllers
{
    public class RegistrationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public RegistrationsController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var registrations = await _context.Registrations
                .Include(r => r.Student)
                .Include(r => r.Module)
                .OrderBy(r => r.Student!.LastName)
                .ThenBy(r => r.Module!.ModuleCode)
                .ToListAsync();
            return View(registrations);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var registration = await _context.Registrations
                .Include(r => r.Student)
                .Include(r => r.Module)
                .FirstOrDefaultAsync(r => r.RegistrationID == id);
            return registration == null ? NotFound() : View(registration);
        }

        public IActionResult Create(int? studentId)
        {
            ViewData["StudentID"] = new SelectList(_context.Students
                .OrderBy(s => s.LastName), "StudentID", "FullName", studentId);
            ViewData["ModuleID"] = new SelectList(_context.Modules
                .OrderBy(m => m.ModuleCode), "ModuleID", "ModuleDisplay");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StudentID,ModuleID")] Registration registration)
        {
            // Prevent duplicate registration
            bool exists = await _context.Registrations
                .AnyAsync(r => r.StudentID == registration.StudentID && r.ModuleID == registration.ModuleID);

            if (exists)
            {
                ModelState.AddModelError("", "This student is already registered for that module.");
            }

            if (ModelState.IsValid)
            {
                registration.DateRegistered = DateTime.Now;
                _context.Add(registration);
                await _context.SaveChangesAsync();

                var student = await _context.Students.FindAsync(registration.StudentID);
                var module = await _context.Modules.FindAsync(registration.ModuleID);
                TempData["Success"] = $"{student?.FirstName} {student?.LastName} registered for {module?.ModuleName} successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["StudentID"] = new SelectList(_context.Students.OrderBy(s => s.LastName), "StudentID", "FullName", registration.StudentID);
            ViewData["ModuleID"] = new SelectList(_context.Modules.OrderBy(m => m.ModuleCode), "ModuleID", "ModuleDisplay", registration.ModuleID);
            return View(registration);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var registration = await _context.Registrations
                .Include(r => r.Student)
                .Include(r => r.Module)
                .FirstOrDefaultAsync(r => r.RegistrationID == id);
            return registration == null ? NotFound() : View(registration);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var registration = await _context.Registrations
                .Include(r => r.Student)
                .Include(r => r.Module)
                .FirstOrDefaultAsync(r => r.RegistrationID == id);

            if (registration != null)
            {
                _context.Registrations.Remove(registration);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"{registration.Student?.FirstName} {registration.Student?.LastName} unregistered from {registration.Module?.ModuleName}.";
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Registrations/ByStudent/5
        public async Task<IActionResult> ByStudent(int? studentId)
        {
            if (studentId == null) return NotFound();

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentID == studentId);

            if (student == null) return NotFound();

            var registrations = await _context.Registrations
                .Include(r => r.Student)
                .Include(r => r.Module)
                .Where(r => r.StudentID == studentId)
                .OrderBy(r => r.Module!.ModuleCode)
                .ToListAsync();

            ViewBag.StudentName = $"{student.FirstName} {student.LastName}";
            return View(registrations);
        }

        // GET: Registrations/ByModule/5
        public async Task<IActionResult> ByModule(int? moduleId)
        {
            if (moduleId == null) return NotFound();

            var module = await _context.Modules
                .FirstOrDefaultAsync(m => m.ModuleID == moduleId);

            if (module == null) return NotFound();

            var registrations = await _context.Registrations
                .Include(r => r.Student)
                .Include(r => r.Module)
                .Where(r => r.ModuleID == moduleId)
                .OrderBy(r => r.Student!.LastName)
                .ToListAsync();

            ViewBag.ModuleName = $"{module.ModuleCode} - {module.ModuleName}";
            return View(registrations);
        }
    }
}