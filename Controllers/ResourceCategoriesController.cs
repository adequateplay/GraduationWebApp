using GraduationWebApp.Data;
using GraduationWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GraduationWebApp.Controllers
{
    public class ResourceCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ResourceCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ResourceCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.ResourceCategories.ToListAsync());
        }

        // GET: ResourceCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceCategories = await _context.ResourceCategories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (resourceCategories == null)
            {
                return NotFound();
            }

            return View(resourceCategories);
        }

        // GET: ResourceCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ResourceCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,CategoryName")] ResourceCategories resourceCategories)
        {
            if (ModelState.IsValid)
            {
                _context.Add(resourceCategories);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(resourceCategories);
        }

        // GET: ResourceCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceCategories = await _context.ResourceCategories.FindAsync(id);
            if (resourceCategories == null)
            {
                return NotFound();
            }
            return View(resourceCategories);
        }

        // POST: ResourceCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CategoryId,CategoryName")] ResourceCategories resourceCategories)
        {
            if (id != resourceCategories.CategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(resourceCategories);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResourceCategoriesExists(resourceCategories.CategoryId))
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
            return View(resourceCategories);
        }

        // GET: ResourceCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resourceCategories = await _context.ResourceCategories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            if (resourceCategories == null)
            {
                return NotFound();
            }

            return View(resourceCategories);
        }

        // POST: ResourceCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resourceCategories = await _context.ResourceCategories.FindAsync(id);
            if (resourceCategories != null)
            {
                _context.ResourceCategories.Remove(resourceCategories);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResourceCategoriesExists(int id)
        {
            return _context.ResourceCategories.Any(e => e.CategoryId == id);
        }
    }
}
