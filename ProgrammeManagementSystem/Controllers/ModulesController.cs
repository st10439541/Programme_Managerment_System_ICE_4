using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammeManagementSystem.Data;
using ProgrammeManagementSystem.Models;

namespace ProgrammeManagementSystem.Controllers
{
    public class ModulesController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ModulesController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
            => View(await _context.Modules
                .Include(m => m.ModuleAssignments)
                .ThenInclude(a => a.Lecturer)
                .Include(m => m.Registrations)
                .OrderBy(m => m.ModuleCode)
                .ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var module = await _context.Modules
                .Include(m => m.Registrations)
                .ThenInclude(r => r.Student)
                .Include(m => m.ModuleAssignments)
                .ThenInclude(a => a.Lecturer)
                .FirstOrDefaultAsync(m => m.ModuleID == id);

            return module == null ? NotFound() : View(module);
        }

        public IActionResult Create() => View();

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ModuleName,ModuleCode,Credits,AcademicYear")] Module module)
        {
            // Check for duplicate module code
            var existingModule = await _context.Modules
                .FirstOrDefaultAsync(m => m.ModuleCode == module.ModuleCode);

            if (existingModule != null)
            {
                ModelState.AddModelError("ModuleCode", "A module with this code already exists.");
            }

            if (!ModelState.IsValid) return View(module);

            _context.Add(module);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Module {module.ModuleCode} - {module.ModuleName} created.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var module = await _context.Modules.FindAsync(id);
            return module == null ? NotFound() : View(module);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ModuleID,ModuleName,ModuleCode,Credits,AcademicYear")] Module module)
        {
            if (id != module.ModuleID) return NotFound();

            // Check for duplicate module code (excluding current)
            var existingModule = await _context.Modules
                .FirstOrDefaultAsync(m => m.ModuleCode == module.ModuleCode && m.ModuleID != module.ModuleID);

            if (existingModule != null)
            {
                ModelState.AddModelError("ModuleCode", "A module with this code already exists.");
            }

            if (!ModelState.IsValid) return View(module);

            _context.Update(module);
            await _context.SaveChangesAsync();
            TempData["Success"] = $"Module {module.ModuleCode} updated.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var module = await _context.Modules
                .Include(m => m.Registrations)
                .Include(m => m.ModuleAssignments)
                .FirstOrDefaultAsync(m => m.ModuleID == id);

            return module == null ? NotFound() : View(module);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var module = await _context.Modules
                .Include(m => m.Registrations)
                .Include(m => m.ModuleAssignments)
                .FirstOrDefaultAsync(m => m.ModuleID == id);

            if (module != null)
            {
                _context.Modules.Remove(module);
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Module {module.ModuleCode} deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}