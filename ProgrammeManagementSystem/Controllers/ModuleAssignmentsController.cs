using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProgrammeManagementSystem.Data;
using ProgrammeManagementSystem.Models;

namespace ProgrammeManagementSystem.Controllers
{
    public class ModuleAssignmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ModuleAssignmentsController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var assignments = await _context.ModuleAssignments
                .Include(a => a.Lecturer)
                .Include(a => a.Module)
                .OrderBy(a => a.Module!.ModuleCode)
                .ToListAsync();
            return View(assignments);
        }

        public IActionResult Create()
        {
            ViewData["LecturerID"] = new SelectList(_context.Lecturers
                .OrderBy(l => l.LastName), "LecturerID", "FullName", null, "LastName");
            ViewData["ModuleID"] = new SelectList(_context.Modules
                .OrderBy(m => m.ModuleCode), "ModuleID", "ModuleDisplay");
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LecturerID,ModuleID")] ModuleAssignment assignment)
        {
            // Check for duplicate assignment
            bool exists = await _context.ModuleAssignments
                .AnyAsync(a => a.LecturerID == assignment.LecturerID && a.ModuleID == assignment.ModuleID);

            if (exists)
            {
                ModelState.AddModelError("", "This lecturer is already assigned to this module.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(assignment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Lecturer assigned to module successfully.";
                return RedirectToAction(nameof(Index));
            }

            ViewData["LecturerID"] = new SelectList(_context.Lecturers.OrderBy(l => l.LastName), "LecturerID", "FullName", assignment.LecturerID);
            ViewData["ModuleID"] = new SelectList(_context.Modules.OrderBy(m => m.ModuleCode), "ModuleID", "ModuleDisplay", assignment.ModuleID);
            return View(assignment);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var assignment = await _context.ModuleAssignments
                .Include(a => a.Lecturer)
                .Include(a => a.Module)
                .FirstOrDefaultAsync(a => a.AssignmentID == id);
            return assignment == null ? NotFound() : View(assignment);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var assignment = await _context.ModuleAssignments.FindAsync(id);
            if (assignment == null) return NotFound();

            ViewData["LecturerID"] = new SelectList(_context.Lecturers.OrderBy(l => l.LastName), "LecturerID", "FullName", assignment.LecturerID);
            ViewData["ModuleID"] = new SelectList(_context.Modules.OrderBy(m => m.ModuleCode), "ModuleID", "ModuleDisplay", assignment.ModuleID);
            return View(assignment);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AssignmentID,LecturerID,ModuleID")] ModuleAssignment assignment)
        {
            if (id != assignment.AssignmentID) return NotFound();

            // Check for duplicate assignment (excluding current)
            bool exists = await _context.ModuleAssignments
                .AnyAsync(a => a.LecturerID == assignment.LecturerID &&
                              a.ModuleID == assignment.ModuleID &&
                              a.AssignmentID != assignment.AssignmentID);

            if (exists)
            {
                ModelState.AddModelError("", "This lecturer is already assigned to this module.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(assignment);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Assignment updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AssignmentExists(assignment.AssignmentID)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["LecturerID"] = new SelectList(_context.Lecturers.OrderBy(l => l.LastName), "LecturerID", "FullName", assignment.LecturerID);
            ViewData["ModuleID"] = new SelectList(_context.Modules.OrderBy(m => m.ModuleCode), "ModuleID", "ModuleDisplay", assignment.ModuleID);
            return View(assignment);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var assignment = await _context.ModuleAssignments
                .Include(a => a.Lecturer)
                .Include(a => a.Module)
                .FirstOrDefaultAsync(a => a.AssignmentID == id);
            return assignment == null ? NotFound() : View(assignment);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var assignment = await _context.ModuleAssignments.FindAsync(id);
            if (assignment != null)
            {
                _context.ModuleAssignments.Remove(assignment);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Assignment removed successfully.";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AssignmentExists(int id)
        {
            return _context.ModuleAssignments.Any(e => e.AssignmentID == id);
        }
    }
}