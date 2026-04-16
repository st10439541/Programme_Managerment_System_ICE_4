using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammeManagementSystem.Data;
using ProgrammeManagementSystem.Models;

namespace ProgrammeManagementSystem.Controllers
{
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .Include(s => s.Registrations)
                .ThenInclude(r => r.Module)
                .OrderBy(s => s.LastName)
                .ToListAsync();
            return View(students);
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Registrations)
                .ThenInclude(r => r.Module)
                .FirstOrDefaultAsync(m => m.StudentID == id);

            if (student == null) return NotFound();

            return View(student);
        }

        // GET: Students/Create
        public IActionResult Create() => View();

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,Email,PhoneNumber,YearOfStudy")] Student student)
        {
            // Check for duplicate email
            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == student.Email);

            if (existingStudent != null)
            {
                ModelState.AddModelError("Email", "A student with this email already exists.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(student);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Student {student.FirstName} {student.LastName} added successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StudentID,FirstName,LastName,Email,PhoneNumber,YearOfStudy")] Student student)
        {
            if (id != student.StudentID) return NotFound();

            // Check for duplicate email (excluding current student)
            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == student.Email && s.StudentID != student.StudentID);

            if (existingStudent != null)
            {
                ModelState.AddModelError("Email", "A student with this email already exists.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = $"Student {student.FirstName} {student.LastName} updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.StudentID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(student);
        }

        // GET: Students/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students
                .Include(s => s.Registrations)
                .FirstOrDefaultAsync(m => m.StudentID == id);

            if (student == null) return NotFound();

            // Show warning if student has registrations
            if (student.Registrations.Any())
            {
                ViewBag.HasRegistrations = true;
                ViewBag.RegistrationCount = student.Registrations.Count;
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students
                .Include(s => s.Registrations)
                .FirstOrDefaultAsync(s => s.StudentID == id);

            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Student {student.FirstName} {student.LastName} deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentID == id);
        }
    }
}