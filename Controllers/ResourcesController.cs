using GraduationWebApp.Data;
using GraduationWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationWebApp.Controllers
{
    public class ResourcesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const int PageSize = 5;

        public ResourcesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int pageNumber = 1)
        {
            var resources = await _context.Resources
                .OrderBy(r => r.ResourceId)
                .Skip((pageNumber - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            ViewBag.PageNumber = pageNumber;
            ViewBag.TotalPages = (int)Math.Ceiling((double)_context.Resources.Count() / PageSize);
            ViewBag.IsAuthenticated = User.Identity.IsAuthenticated && (User.IsInRole("Психолог") || User.IsInRole("Админ"));

            return View(resources);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resources = await _context.Resources
                .FirstOrDefaultAsync(m => m.ResourceId == id);
            if (resources == null)
            {
                return NotFound();
            }

            return View(resources);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ResourceId,Name,Description,ResourceType,ResourceFile")] Resources resources)
        {
            if (ModelState.IsValid)
            {
                _context.Add(resources);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(resources);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resources = await _context.Resources.FindAsync(id);
            if (resources == null)
            {
                return NotFound();
            }
            return View(resources);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ResourceId,Name,Description,ResourceType,ResourceFile")] Resources resources)
        {
            if (id != resources.ResourceId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(resources);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResourcesExists(resources.ResourceId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(resources);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resources = await _context.Resources
                .FirstOrDefaultAsync(m => m.ResourceId == id);
            if (resources == null)
            {
                return NotFound();
            }

            return View(resources);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resources = await _context.Resources.FindAsync(id);
            if (resources != null)
            {
                _context.Resources.Remove(resources);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResourcesExists(int id)
        {
            return _context.Resources.Any(e => e.ResourceId == id);
        }
    }
}
